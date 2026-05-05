using IdentityEmail.Context;
using IdentityEmail.Dtos;
using IdentityEmail.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace IdentityEmail.Controllers
{
    [Authorize]
    public class MessageController : LayoutController
    {
        public MessageController(EmailContext context, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
            : base(context, userManager, signInManager)
        {
        }

        public async Task<IActionResult> Inbox(int page = 1, string search = "", string unreadOnly = "false", string folder = "inbox", int? categoryId = null)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(userEmail)) return RedirectToAction("Login", "Account");

            var user = await _userManager.FindByEmailAsync(userEmail);
            if (user == null) return RedirectToAction("Login", "Account");

            await FillLayoutData();

            bool isUnreadOnly = unreadOnly == "true";
            int pageSize = 10;

            var spamSenders = _context.SpamSenders
                .Where(s => s.UserEmail.ToLower() == userEmail.ToLower())
                .Select(s => s.SenderEmail.ToLower())
                .ToList();

            var baseQuery = _context.UserMessageBoxes
                .Where(umb => umb.UserMessageBoxEmail.ToLower() == userEmail.ToLower());

            IQueryable<UserMessageBox> query;

            switch (folder)
            {
                case "starred":
                    query = baseQuery.Where(umb => umb.IsStarred && !umb.IsTrash);
                    break;
                case "sent":
                    query = baseQuery.Where(umb =>
                        umb.Message.SenderEmail.ToLower() == userEmail.ToLower() &&
                        !umb.IsDraft && !umb.IsTrash);
                    break;
                case "draft":
                    query = baseQuery.Where(umb =>
                        umb.IsDraft &&
                        umb.Message.SenderEmail.ToLower() == userEmail.ToLower());
                    break;
                case "spam":
                    query = baseQuery.Where(umb =>
                        !umb.IsTrash &&
                        (umb.IsSpam || spamSenders.Contains(umb.Message.SenderEmail.ToLower())));
                    break;
                case "trash":
                    query = baseQuery.Where(umb => umb.IsTrash);
                    break;
                case "category":
                    query = baseQuery.Where(umb =>
                        umb.CategoryId == categoryId &&
                        !umb.IsTrash && !umb.IsSpam);
                    break;
                default:
                    query = baseQuery.Where(umb =>
                        !umb.IsDraft && !umb.IsTrash && !umb.IsSpam &&
                        umb.Message.ReceiverEmail.ToLower() == userEmail.ToLower() &&
                        !spamSenders.Contains(umb.Message.SenderEmail.ToLower()));
                    break;
            }

            if (!string.IsNullOrEmpty(search))
                query = query.Where(umb =>
                    umb.Message.Subject.Contains(search) ||
                    umb.Message.MessageDetail.Contains(search) ||
                    umb.Message.SenderEmail.Contains(search));

            if (isUnreadOnly)
                query = query.Where(umb => !umb.IsRead);

            int totalCount = query.Count();

            var values = query
                .Select(umb => new InboxDto
                {
                    MessageId = umb.Message.MessageId,
                    Subject = umb.Message.Subject,
                    SendDate = umb.Message.SendDate,
                    IsRead = umb.IsRead,
                    IsStarred = umb.IsStarred,
                    IsDraft = umb.IsDraft,
                    IsTrash = umb.IsTrash,
                    IsSpam = umb.IsSpam,
                    CategoryId = umb.Category != null ? umb.Category.CategoryId : (int?)null,
                    CategoryName = umb.Category != null ? umb.Category.Name : null,
                    CategoryColor = umb.Category != null ? umb.Category.Color : null,
                    ImageUrl = _context.Users
                        .Where(u => u.Email.ToLower() == umb.Message.SenderEmail.ToLower())
                        .Select(u => u.ImageUrl)
                        .FirstOrDefault(),
                    SenderFullname = _context.Users
                        .Where(u => u.Email.ToLower() == umb.Message.SenderEmail.ToLower())
                        .Select(u => u.Name + " " + u.Surname)
                        .FirstOrDefault() ?? "Bilinmeyen Kullanıcı"
                })
                .OrderByDescending(x => x.SendDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            ViewBag.Search = search;
            ViewBag.UnreadOnly = isUnreadOnly;
            ViewBag.CurrentFolder = folder;
            ViewBag.CurrentCategory = categoryId;

            return View(values);
        }

        public async Task<IActionResult> MessageDetail(int id, string returnUrl = "/Message/Inbox")
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(userEmail)) return RedirectToAction("Login", "Account");

            var user = await _userManager.FindByEmailAsync(userEmail);
            if (user == null) return RedirectToAction("Login", "Account");

            var umb = _context.UserMessageBoxes
                .Include(u => u.Message)
                .Include(u => u.Category)
                .FirstOrDefault(u =>
                    u.UserMessageBoxEmail.ToLower() == userEmail.ToLower() &&
                    u.MessageId == id);

            if (umb == null) return RedirectToAction("Inbox");

            if (umb.IsDraft)
                return RedirectToAction("EditDraft", new { id = umb.MessageId, returnUrl });

            await FillLayoutData();

            if (!umb.IsRead)
            {
                umb.IsRead = true;
                await _context.SaveChangesAsync();
            }

            var senderUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == umb.Message.SenderEmail.ToLower());

            var dto = new MessageDetailDto
            {
                MessageId = umb.Message.MessageId,
                Subject = umb.Message.Subject,
                MessageDetail = umb.Message.MessageDetail,
                SendDate = umb.Message.SendDate,
                SenderEmail = umb.Message.SenderEmail,
                ReceiverEmail = umb.Message.ReceiverEmail,
                IsRead = umb.IsRead,
                IsStarred = umb.IsStarred,
                IsTrash = umb.IsTrash,
                IsSpam = umb.IsSpam,
                IsDraft = umb.IsDraft,
                CategoryId = umb.Category?.CategoryId,
                CategoryName = umb.Category?.Name,
                CategoryColor = umb.Category?.Color,
                SenderFullname = senderUser != null
                    ? senderUser.Name + " " + senderUser.Surname
                    : "Bilinmeyen Kullanıcı",
                SenderImageUrl = senderUser?.ImageUrl
            };

            string detailFolder = "inbox";
            if (returnUrl.Contains("folder=sent")) detailFolder = "sent";
            else if (returnUrl.Contains("folder=draft")) detailFolder = "draft";
            else if (returnUrl.Contains("folder=starred")) detailFolder = "starred";
            else if (returnUrl.Contains("folder=spam")) detailFolder = "spam";
            else if (returnUrl.Contains("folder=trash")) detailFolder = "trash";
            else if (returnUrl.Contains("folder=category")) detailFolder = "category";

            int? detailCategoryId = null;
            if (returnUrl.Contains("categoryId="))
            {
                var part = returnUrl.Split("categoryId=")[1].Split("&")[0];
                if (int.TryParse(part, out int parsedId))
                    detailCategoryId = parsedId;
            }

            var contactCat = _context.UserContactCategories
                .FirstOrDefault(u =>
                    u.UserEmail.ToLower() == userEmail.ToLower() &&
                    u.ContactEmail.ToLower() == umb.Message.SenderEmail.ToLower());

            ViewBag.CurrentFolder = detailFolder;
            ViewBag.CurrentCategory = detailCategoryId;
            ViewBag.ReturnUrl = returnUrl;
            ViewBag.ContactCategoryId = contactCat?.CategoryId;

            return View(dto);
        }

        public async Task<IActionResult> EditDraft(int id, string returnUrl = "/Message/Inbox?folder=draft")
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(userEmail)) return RedirectToAction("Login", "Account");

            await FillLayoutData();

            var message = await _context.Messages.FindAsync(id);
            if (message == null) return RedirectToAction("Inbox");

            var umb = _context.UserMessageBoxes
                .FirstOrDefault(u =>
                    u.MessageId == id &&
                    u.UserMessageBoxEmail.ToLower() == userEmail.ToLower() &&
                    u.IsDraft);

            if (umb == null) return RedirectToAction("Inbox");

            if (!umb.IsRead)
            {
                umb.IsRead = true;
                await _context.SaveChangesAsync();
            }

            ViewBag.DraftMessageId = id;
            ViewBag.ReturnUrl = returnUrl;
            ViewBag.CurrentFolder = "draft";
            ViewBag.CurrentCategory = null;
            return View("CreateMessage", message);
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("UserLogin", "Login");
        }

        [HttpGet]
        public async Task<IActionResult> CreateMessage()
        {
            await FillLayoutData();
            ViewBag.DraftMessageId = 0;
            return View(new Message());
        }

        [HttpPost]
        public async Task<IActionResult> CreateMessage(string receiverEmail, string subject, string messageDetail, int draftMessageId = 0)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.FindByEmailAsync(userEmail);
            if (user == null) return Json(new { success = false });

            var receiver = await _userManager.FindByEmailAsync(receiverEmail);
            if (receiver == null)
                return Json(new { success = false, message = "Bu e-posta adresine kayıtlı kullanıcı bulunamadı." });

            if (draftMessageId > 0)
            {
                var oldUmb = _context.UserMessageBoxes
                    .Where(u => u.MessageId == draftMessageId).ToList();
                if (oldUmb.Any()) _context.UserMessageBoxes.RemoveRange(oldUmb);

                var oldDraft = await _context.Messages.FindAsync(draftMessageId);
                if (oldDraft != null) _context.Messages.Remove(oldDraft);
            }

            var message = new Message
            {
                SenderEmail = userEmail,
                ReceiverEmail = receiverEmail,
                Subject = subject,
                MessageDetail = messageDetail,
                SendDate = DateTime.Now
            };

            await _context.Messages.AddAsync(message);
            await _context.SaveChangesAsync();

            if (receiverEmail.ToLower() == userEmail.ToLower())
            {
                var selfContactCat = _context.UserContactCategories
                    .FirstOrDefault(u =>
                        u.UserEmail.ToLower() == userEmail.ToLower() &&
                        u.ContactEmail.ToLower() == userEmail.ToLower());

                _context.UserMessageBoxes.Add(new UserMessageBox
                {
                    UserMessageBoxEmail = userEmail,
                    MessageId = message.MessageId,
                    IsDraft = false,
                    IsRead = false,
                    CategoryId = selfContactCat?.CategoryId
                });
            }
            else
            {
                _context.UserMessageBoxes.Add(new UserMessageBox
                {
                    UserMessageBoxEmail = userEmail,
                    MessageId = message.MessageId,
                    IsDraft = false,
                    IsRead = true
                });

                var receiverContactCat = _context.UserContactCategories
                    .FirstOrDefault(u =>
                        u.UserEmail.ToLower() == receiverEmail.ToLower() &&
                        u.ContactEmail.ToLower() == userEmail.ToLower());

                _context.UserMessageBoxes.Add(new UserMessageBox
                {
                    UserMessageBoxEmail = receiverEmail,
                    MessageId = message.MessageId,
                    IsDraft = false,
                    IsRead = false,
                    CategoryId = receiverContactCat?.CategoryId
                });
            }

            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> SaveDraft(string receiverEmail, string subject, string messageDetail, int draftMessageId = 0)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.FindByEmailAsync(userEmail);
            if (user == null) return Json(new { success = false });

            if (draftMessageId > 0)
            {
                var existing = await _context.Messages.FindAsync(draftMessageId);
                if (existing != null)
                {
                    existing.ReceiverEmail = receiverEmail ?? "";
                    existing.Subject = subject ?? "";
                    existing.MessageDetail = messageDetail ?? "";
                    existing.SendDate = DateTime.Now;
                    await _context.SaveChangesAsync();
                    return Json(new { success = true });
                }
            }

            var message = new Message
            {
                SenderEmail = userEmail,
                ReceiverEmail = receiverEmail ?? "",
                Subject = subject ?? "",
                MessageDetail = messageDetail ?? "",
                SendDate = DateTime.Now
            };

            await _context.Messages.AddAsync(message);
            await _context.SaveChangesAsync();

            _context.UserMessageBoxes.Add(new UserMessageBox
            {
                UserMessageBoxEmail = userEmail,
                MessageId = message.MessageId,
                IsDraft = true,
                IsRead = true
            });

            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> AddCategory(string name, string color)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            var count = _context.MessageCategory
                .Count(c => c.UserEmail.ToLower() == userEmail.ToLower());

            if (count >= 5)
                return Json(new { success = false, message = "En fazla 5 kategori ekleyebilirsiniz." });

            _context.MessageCategory.Add(new MessageCategory
            {
                Name = name,
                Color = color,
                UserEmail = userEmail
            });

            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCategory(int categoryId)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);

            var category = _context.MessageCategory
                .FirstOrDefault(c =>
                    c.CategoryId == categoryId &&
                    c.UserEmail.ToLower() == userEmail.ToLower());

            if (category == null)
                return Json(new { success = false, message = "Kategori bulunamadı." });

            var boxes = _context.UserMessageBoxes
                .Where(u => u.CategoryId == categoryId).ToList();
            boxes.ForEach(u => u.CategoryId = null);

            var contactCategories = _context.UserContactCategories
                .Where(u => u.CategoryId == categoryId).ToList();
            _context.UserContactCategories.RemoveRange(contactCategories);

            _context.MessageCategory.Remove(category);
            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> ToggleStar(int messageId)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            var umb = _context.UserMessageBoxes
                .Include(u => u.Message)
                .FirstOrDefault(u =>
                    u.MessageId == messageId &&
                    u.UserMessageBoxEmail.ToLower() == userEmail.ToLower());

            if (umb == null) return Json(new { success = false });

            umb.IsStarred = !umb.IsStarred;
            await _context.SaveChangesAsync();
            return Json(new { success = true, isStarred = umb.IsStarred });
        }

        [HttpPost]
        public async Task<IActionResult> BulkAction(List<int> messageIds, string action, int? categoryId = null)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            var boxes = _context.UserMessageBoxes
                .Include(u => u.Message)
                .Where(u =>
                    u.UserMessageBoxEmail.ToLower() == userEmail.ToLower() &&
                    messageIds.Contains(u.MessageId))
                .ToList();

            if (!boxes.Any()) return Json(new { success = false });

            switch (action)
            {
                case "trash":
                    foreach (var box in boxes)
                    {
                        if (box.IsDraft)
                        {
                            var otherBoxes = _context.UserMessageBoxes
                                .Where(u => u.MessageId == box.MessageId &&
                                            u.UserMessageBoxEmail.ToLower() != userEmail.ToLower())
                                .ToList();
                            _context.UserMessageBoxes.Remove(box);
                            if (!otherBoxes.Any())
                                _context.Messages.Remove(box.Message);
                        }
                        else
                        {
                            box.IsTrash = true;
                            box.IsSpam = false;
                        }
                    }
                    break;
                case "untrash":
                    boxes.ForEach(u => u.IsTrash = false);
                    break;
                case "spam":
                    boxes.ForEach(u => { u.IsSpam = true; u.IsTrash = false; });
                    var spamEmailsToAdd = boxes
                        .Select(b => b.Message.SenderEmail.ToLower()).Distinct()
                        .Where(e => !_context.SpamSenders.Any(s =>
                            s.UserEmail.ToLower() == userEmail.ToLower() &&
                            s.SenderEmail.ToLower() == e))
                        .ToList();
                    foreach (var email in spamEmailsToAdd)
                        _context.SpamSenders.Add(new SpamSender
                        {
                            UserEmail = userEmail,
                            SenderEmail = email,
                            CreatedAt = DateTime.Now
                        });
                    break;
                case "unspam":
                    boxes.ForEach(u => u.IsSpam = false);
                    var spamEmailsToRemove = boxes
                        .Select(b => b.Message.SenderEmail.ToLower()).Distinct().ToList();
                    var spamEntries = _context.SpamSenders
                        .Where(s => s.UserEmail.ToLower() == userEmail.ToLower() &&
                            spamEmailsToRemove.Contains(s.SenderEmail.ToLower())).ToList();
                    _context.SpamSenders.RemoveRange(spamEntries);
                    break;
                case "markread":
                    boxes.ForEach(u => u.IsRead = true);
                    break;
                case "markunread":
                    boxes.ForEach(u => u.IsRead = false);
                    break;
                case "categorize":
                    boxes.ForEach(u => u.CategoryId = categoryId == 0 ? null : categoryId);
                    break;
                case "star":
                    boxes.ForEach(u => u.IsStarred = true);
                    break;
            }

            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> PermanentDelete(List<int> messageIds)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);

            var boxes = _context.UserMessageBoxes
                .Include(u => u.Message)
                .Where(u =>
                    u.UserMessageBoxEmail.ToLower() == userEmail.ToLower() &&
                    u.IsTrash &&
                    messageIds.Contains(u.MessageId))
                .ToList();

            if (!boxes.Any()) return Json(new { success = false });

            foreach (var box in boxes)
            {
                var otherBoxes = _context.UserMessageBoxes
                    .Where(u =>
                        u.MessageId == box.MessageId &&
                        u.UserMessageBoxEmail.ToLower() != userEmail.ToLower())
                    .ToList();

                _context.UserMessageBoxes.Remove(box);

                if (!otherBoxes.Any())
                    _context.Messages.Remove(box.Message);
            }

            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> ReplyMessage(int originalMessageId, string content, bool isDraft = false)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.FindByEmailAsync(userEmail);
            if (user == null) return Json(new { success = false });

            var original = await _context.Messages.FindAsync(originalMessageId);
            if (original == null)
                return Json(new { success = false, message = "Orijinal mesaj bulunamadı." });

            var reply = new Message
            {
                SenderEmail = userEmail,
                ReceiverEmail = original.SenderEmail,
                Subject = "Re: " + original.Subject,
                MessageDetail = content,
                SendDate = DateTime.Now
            };

            await _context.Messages.AddAsync(reply);
            await _context.SaveChangesAsync();

            _context.UserMessageBoxes.Add(new UserMessageBox
            {
                UserMessageBoxEmail = userEmail,
                MessageId = reply.MessageId,
                IsDraft = isDraft,
                IsRead = !isDraft
            });

            if (!isDraft && original.SenderEmail.ToLower() != userEmail.ToLower())
            {
                var replyReceiverCat = _context.UserContactCategories
                    .FirstOrDefault(u =>
                        u.UserEmail.ToLower() == original.SenderEmail.ToLower() &&
                        u.ContactEmail.ToLower() == userEmail.ToLower());

                _context.UserMessageBoxes.Add(new UserMessageBox
                {
                    UserMessageBoxEmail = original.SenderEmail,
                    MessageId = reply.MessageId,
                    IsDraft = false,
                    IsRead = false,
                    CategoryId = replyReceiverCat?.CategoryId
                });
            }

            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> SetContactCategory(string contactEmail, int? categoryId)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);

            var existing = _context.UserContactCategories
                .FirstOrDefault(u =>
                    u.UserEmail.ToLower() == userEmail.ToLower() &&
                    u.ContactEmail.ToLower() == contactEmail.ToLower());

            if (categoryId == null || categoryId == 0)
            {
                if (existing != null)
                {
                    _context.UserContactCategories.Remove(existing);
                    await _context.SaveChangesAsync();
                }
                return Json(new { success = true });
            }

            var category = _context.MessageCategory
                .FirstOrDefault(c =>
                    c.CategoryId == categoryId &&
                    c.UserEmail.ToLower() == userEmail.ToLower());

            if (category == null)
                return Json(new { success = false, message = "Kategori bulunamadı." });

            if (existing != null)
            {
                existing.CategoryId = categoryId.Value;
            }
            else
            {
                _context.UserContactCategories.Add(new UserContactCategory
                {
                    UserEmail = userEmail,
                    ContactEmail = contactEmail,
                    CategoryId = categoryId.Value
                });
            }

            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }
    }
}