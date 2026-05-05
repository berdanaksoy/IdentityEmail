using IdentityEmail.Context;
using IdentityEmail.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace IdentityEmail.Controllers
{
    public class LayoutController : Controller
    {
        protected readonly EmailContext _context;
        protected readonly UserManager<AppUser> _userManager;
        protected readonly SignInManager<AppUser> _signInManager;

        public LayoutController(EmailContext context, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        protected async Task FillLayoutData()
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(userEmail)) return;

            var user = await _userManager.FindByEmailAsync(userEmail);
            if (user == null) return;

            var spamSenders = _context.SpamSenders
                .Where(s => s.UserEmail.ToLower() == userEmail.ToLower())
                .Select(s => s.SenderEmail.ToLower())
                .ToList();

            ViewBag.LoggedFullname = user.Name + " " + user.Surname;
            ViewBag.LoggedEmail = user.Email;
            ViewBag.LoggedImageUrl = user.ImageUrl;

            ViewBag.Categories = _context.MessageCategory
                .Where(c => c.UserEmail.ToLower() == userEmail.ToLower())
                .ToList();

            ViewBag.UnreadCount = _context.UserMessageBoxes.Count(u =>
                u.UserMessageBoxEmail.ToLower() == userEmail.ToLower() &&
                !u.IsRead && !u.IsDraft && !u.IsTrash && !u.IsSpam &&
                !spamSenders.Contains(u.Message.SenderEmail.ToLower()));

            ViewBag.DraftCount = _context.UserMessageBoxes.Count(u =>
                u.UserMessageBoxEmail.ToLower() == userEmail.ToLower() &&
                u.IsDraft && u.Message.SenderEmail.ToLower() == userEmail.ToLower());

            ViewBag.SpamCount = _context.UserMessageBoxes.Count(u =>
                u.UserMessageBoxEmail.ToLower() == userEmail.ToLower() &&
                !u.IsTrash && (u.IsSpam || spamSenders.Contains(u.Message.SenderEmail.ToLower())));

            ViewBag.TrashCount = _context.UserMessageBoxes.Count(u =>
                u.UserMessageBoxEmail.ToLower() == userEmail.ToLower() && u.IsTrash);

            ViewBag.UnreadMessages = _context.UserMessageBoxes
                .Where(u =>
                    u.UserMessageBoxEmail.ToLower() == userEmail.ToLower() &&
                    !u.IsRead && !u.IsDraft && !u.IsTrash && !u.IsSpam &&
                    !spamSenders.Contains(u.Message.SenderEmail.ToLower()))
                .OrderByDescending(u => u.Message.SendDate)
                .Take(5)
                .Select(u => new
                {
                    u.Message.MessageId,
                    u.Message.Subject,
                    u.Message.SendDate,
                    SenderFullname = _context.Users
                        .Where(x => x.Email.ToLower() == u.Message.SenderEmail.ToLower())
                        .Select(x => x.Name + " " + x.Surname)
                        .FirstOrDefault() ?? "Bilinmeyen Kullanıcı",
                    ImageUrl = _context.Users
                        .Where(x => x.Email.ToLower() == u.Message.SenderEmail.ToLower())
                        .Select(x => x.ImageUrl)
                        .FirstOrDefault()
                })
                .ToList();
        }
    }
}