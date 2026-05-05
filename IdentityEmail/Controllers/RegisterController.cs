using IdentityEmail.Dtos;
using IdentityEmail.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Mail;
using System.Text.Json;

namespace IdentityEmail.Controllers
{
    public class RegisterController : Controller
    {
        private readonly UserManager<AppUser> _userManager;

        public RegisterController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult CreateUser() => View();

        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserRegisterDto model)
        {
            AppUser tempUser = new()
            {
                Name = model.Name,
                Surname = model.Surname,
                UserName = model.Username,
                Email = model.Email
            };

            if (!string.IsNullOrEmpty(model.Password))
            {
                foreach (var validator in _userManager.PasswordValidators)
                {
                    var result = await validator.ValidateAsync(_userManager, tempUser, model.Password);
                    if (!result.Succeeded)
                        foreach (var e in result.Errors)
                            ModelState.AddModelError(string.Empty, e.Description);
                }
            }

            if (!string.IsNullOrEmpty(model.Username) || !string.IsNullOrEmpty(model.Email))
            {
                foreach (var validator in _userManager.UserValidators)
                {
                    var result = await validator.ValidateAsync(_userManager, tempUser);
                    if (!result.Succeeded)
                        foreach (var e in result.Errors.Where(e => e.Code != "InvalidEmail"))
                            ModelState.AddModelError(string.Empty, e.Description);
                }
            }

            if (!ModelState.IsValid)
                return View(model);

            string code = new Random().Next(100000, 1000000).ToString();

            AppUser appUser = new()
            {
                Name = model.Name,
                Surname = model.Surname,
                UserName = model.Username,
                Email = model.Email,
                ConfirmCode = code,
                EmailConfirmed = false
            };

            var createResult = await _userManager.CreateAsync(appUser, model.Password);
            if (!createResult.Succeeded)
            {
                foreach (var e in createResult.Errors)
                    ModelState.AddModelError(string.Empty, e.Description);
                return View(model);
            }

            HttpContext.Session.SetString("VerifyCode", code);
            HttpContext.Session.SetString("PendingUserId", appUser.Id);

            try
            {
                var smtp = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential("berdan0227@gmail.com", "your code"),
                    EnableSsl = true
                };

                string emailBody = $@"
<!DOCTYPE html>
<html>
<body style='margin:0;padding:0;background:#f9f0f5;font-family:Arial,sans-serif;'>
  <table width='100%' cellpadding='0' cellspacing='0' style='padding:40px 0;'>
    <tr>
      <td align='center'>
        <table width='480' cellpadding='0' cellspacing='0' style='background:#ffffff;border-radius:12px;overflow:hidden;box-shadow:0 4px 20px rgba(0,0,0,0.08);'>
          <tr>
            <td style='background:linear-gradient(135deg,#f953c6,#b91d73);padding:36px;text-align:center;'>
              <h1 style='color:#ffffff;margin:0;font-size:26px;letter-spacing:1px;'>MailMate</h1>
              <p style='color:rgba(255,255,255,0.85);margin:8px 0 0;font-size:14px;'>E-posta Doğrulama</p>
            </td>
          </tr>
          <tr>
            <td style='padding:40px 48px;'>
              <p style='color:#333;font-size:16px;margin:0 0 8px;'>Merhaba <strong>{model.Name}</strong>,</p>
              <p style='color:#666;font-size:14px;line-height:1.6;margin:0 0 32px;'>
                MailMate hesabınızı doğrulamak için aşağıdaki 6 haneli kodu girin.
              </p>
              <div style='background:#fdf0f9;border:2px dashed #f953c6;border-radius:10px;padding:24px;text-align:center;margin-bottom:32px;'>
                <p style='margin:0 0 6px;color:#888;font-size:12px;text-transform:uppercase;letter-spacing:1px;'><strong>Doğrulama Kodunuz<strong></p>
                <p style='margin:0;font-size:42px;font-weight:bold;letter-spacing:12px;color:#b91d73;'>{code}</p>
              </div>
              <p style='color:#999;font-size:12px;margin:0;line-height:1.6;'>
                Bu kodu siz talep etmediyseniz bu e-postayı dikkate almayınız.
              </p>
            </td>
          </tr>
          <tr>
            <td style='background:#fdf0f9;padding:20px 48px;border-top:1px solid #f5c6e8;text-align:center;'>
              <p style='color:#bbb;font-size:12px;margin:0;'>© {DateTime.Now.Year} MailMate. Tüm hakları saklıdır.</p>
            </td>
          </tr>
        </table>
      </td>
    </tr>
  </table>
</body>
</html>";

                var mail = new MailMessage
                {
                    From = new MailAddress("berdan0227@gmail.com", "MailMate"),
                    Subject = "MailMate Doğrulama Kodunuz",
                    Body = emailBody,
                    IsBodyHtml = true
                };
                mail.To.Add(model.Email);
                await smtp.SendMailAsync(mail);
            }
            catch
            {
                ModelState.AddModelError(string.Empty, "Mail gönderilemedi. Lütfen tekrar deneyin.");
                return View(model);
            }

            ViewBag.ShowModal = true;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> VerifyCode(string code)
        {
            var storedCode = HttpContext.Session.GetString("VerifyCode");
            var userId = HttpContext.Session.GetString("PendingUserId");

            if (storedCode == null || userId == null || storedCode != code)
            {
                ViewBag.ShowModal = true;
                ViewBag.VerifyError = "Girdiğiniz kod hatalı. Lütfen tekrar deneyin.";
                
                var pendingUser = await _userManager.FindByIdAsync(userId ?? "");
                if (pendingUser != null)
                {
                    var dto = new CreateUserRegisterDto
                    {
                        Name = pendingUser.Name,
                        Surname = pendingUser.Surname,
                        Username = pendingUser.UserName,
                        Email = pendingUser.Email
                    };
                    return View("CreateUser", dto);
                }
                return RedirectToAction("CreateUser");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return RedirectToAction("CreateUser");

            user.EmailConfirmed = true;
            await _userManager.UpdateAsync(user);

            HttpContext.Session.Clear();
            return RedirectToAction("UserLogin", "Login");
        }
    }
}