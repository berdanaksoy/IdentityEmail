using IdentityEmail.Context;
using IdentityEmail.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace IdentityEmail.Controllers
{
    [Authorize]
    public class ProfileController : LayoutController
    {
        public ProfileController(EmailContext context, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
            : base(context, userManager, signInManager)
        {
        }

        public async Task<IActionResult> Index()
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.FindByEmailAsync(userEmail);
            if (user == null) return RedirectToAction("Login", "Account");

            await FillLayoutData();

            var spamSenders = _context.SpamSenders
                .Where(s => s.UserEmail.ToLower() == userEmail.ToLower())
                .Select(s => s.SenderEmail.ToLower())
                .ToList();

            // İstatistikler
            ViewBag.SentCount = _context.Messages
                .Count(m => m.SenderEmail.ToLower() == userEmail.ToLower());

            ViewBag.ReceivedCount = _context.Messages
                .Count(m => m.ReceiverEmail.ToLower() == userEmail.ToLower());

            ViewBag.UnreadCountStat = _context.UserMessageBoxes
                .Count(u => u.UserMessageBoxEmail.ToLower() == userEmail.ToLower()
                    && !u.IsRead && !u.IsDraft && !u.IsTrash && !u.IsSpam
                    && !spamSenders.Contains(u.Message.SenderEmail.ToLower()));

            ViewBag.StarredCount = _context.UserMessageBoxes
                .Count(u => u.UserMessageBoxEmail.ToLower() == userEmail.ToLower()
                    && u.IsStarred && !u.IsTrash);

            ViewBag.DraftCountStat = _context.UserMessageBoxes
                .Count(u => u.UserMessageBoxEmail.ToLower() == userEmail.ToLower()
                    && u.IsDraft
                    && u.Message.SenderEmail.ToLower() == userEmail.ToLower());

            ViewBag.SpamCountStat = _context.UserMessageBoxes
                .Count(u => u.UserMessageBoxEmail.ToLower() == userEmail.ToLower()
                    && !u.IsTrash
                    && (u.IsSpam || spamSenders.Contains(u.Message.SenderEmail.ToLower())));

            ViewBag.TrashCountStat = _context.UserMessageBoxes
                .Count(u => u.UserMessageBoxEmail.ToLower() == userEmail.ToLower()
                    && u.IsTrash);

            ViewBag.CategoryCount = _context.MessageCategory
                .Count(c => c.UserEmail.ToLower() == userEmail.ToLower());

            // Son 12 ay mesaj dağılımı
            var twelveMonthsAgo = DateTime.Now.AddMonths(-11);
            var startDate = new DateTime(twelveMonthsAgo.Year, twelveMonthsAgo.Month, 1);

            var monthlyMessages = _context.UserMessageBoxes
                .Where(u =>
                    u.UserMessageBoxEmail.ToLower() == userEmail.ToLower() &&
                    !u.IsDraft && !u.IsTrash && !u.IsSpam &&
                    u.Message.ReceiverEmail.ToLower() == userEmail.ToLower() &&
                    u.Message.SendDate >= startDate)
                .GroupBy(u => new { u.Message.SendDate.Year, u.Message.SendDate.Month })
                .Select(g => new { g.Key.Year, g.Key.Month, Count = g.Count() })
                .ToList();

            // 12 aylık diziyi doldur (veri olmayan aylar 0 olsun)
            var monthLabels = new List<string>();
            var monthCounts = new List<int>();

            for (int i = 11; i >= 0; i--)
            {
                var date = DateTime.Now.AddMonths(-i);
                var label = date.ToString("MMM yyyy", new System.Globalization.CultureInfo("tr-TR"));
                var count = monthlyMessages
                    .FirstOrDefault(m => m.Year == date.Year && m.Month == date.Month)?.Count ?? 0;
                monthLabels.Add(label);
                monthCounts.Add(count);
            }

            ViewBag.MonthLabels = System.Text.Json.JsonSerializer.Serialize(monthLabels);
            ViewBag.MonthCounts = System.Text.Json.JsonSerializer.Serialize(monthCounts);

            // Kategori dağılımı
            var categoryStats = _context.UserMessageBoxes
                .Where(u =>
                    u.UserMessageBoxEmail.ToLower() == userEmail.ToLower() &&
                    u.CategoryId != null && !u.IsTrash)
                .GroupBy(u => new { u.Category.Name, u.Category.Color })
                .Select(g => new { g.Key.Name, g.Key.Color, Count = g.Count() })
                .ToList();

            ViewBag.CategoryLabels = System.Text.Json.JsonSerializer.Serialize(categoryStats.Select(c => c.Name).ToList());
            ViewBag.CategoryCounts = System.Text.Json.JsonSerializer.Serialize(categoryStats.Select(c => c.Count).ToList());
            ViewBag.CategoryColors = System.Text.Json.JsonSerializer.Serialize(categoryStats.Select(c => c.Color ?? "#aaa").ToList());

            ViewBag.CurrentFolder = "";
            ViewBag.CurrentCategory = null;

            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfile(string name, string surname, string phoneNumber, string imageUrl)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.FindByEmailAsync(userEmail);
            if (user == null) return Json(new { success = false });

            user.Name = name;
            user.Surname = surname;
            user.PhoneNumber = phoneNumber;
            if (!string.IsNullOrEmpty(imageUrl))
                user.ImageUrl = imageUrl;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
                return Json(new { success = true });

            return Json(new { success = false, message = string.Join(", ", result.Errors.Select(e => e.Description)) });
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(string currentPassword, string newPassword, string confirmPassword)
        {
            if (newPassword != confirmPassword)
                return Json(new { success = false, message = "Yeni şifreler eşleşmiyor." });

            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.FindByEmailAsync(userEmail);
            if (user == null) return Json(new { success = false });

            var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
            if (result.Succeeded)
                return Json(new { success = true });

            var errors = result.Errors.Select(e => e.Code switch
            {
                "PasswordMismatch" => "Mevcut şifre yanlış.",
                "PasswordTooShort" => "Şifre en az 6 karakter olmalıdır.",
                "PasswordRequiresNonAlphanumeric" => "Şifre en az bir özel karakter içermelidir.",
                "PasswordRequiresDigit" => "Şifre en az bir rakam içermelidir.",
                "PasswordRequiresUpper" => "Şifre en az bir büyük harf içermelidir.",
                _ => "Şifre değiştirilemedi."
            });

            return Json(new { success = false, message = string.Join(" ", errors) });
        }
    }
}