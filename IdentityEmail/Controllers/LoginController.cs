using IdentityEmail.Dtos;
using IdentityEmail.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Mail;

namespace IdentityEmail.Controllers
{
    public class LoginController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;

        public LoginController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult UserLogin() => View();

        [HttpPost]
        public async Task<IActionResult> UserLogin(LoginUserDto loginUserDto)
        {
            var user = await _userManager.FindByNameAsync(loginUserDto.Username);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Kullanıcı adı veya şifre hatalı.");
                return View(loginUserDto);
            }

            var passwordCheck = await _signInManager.CheckPasswordSignInAsync(user, loginUserDto.Password, false);
            if (!passwordCheck.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Kullanıcı adı veya şifre hatalı.");
                return View(loginUserDto);
            }

            if (!user.EmailConfirmed)
            {
                string code = new Random().Next(100000, 1000000).ToString();
                user.ConfirmCode = code;
                await _userManager.UpdateAsync(user);

                HttpContext.Session.SetString("LoginVerifyCode", code);
                HttpContext.Session.SetString("LoginUserId", user.Id);

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
              <p style='color:#333;font-size:16px;margin:0 0 8px;'>Merhaba <strong>{user.Name}</strong>,</p>
              <p style='color:#666;font-size:14px;line-height:1.6;margin:0 0 32px;'>
                MailMate hesabınızı doğrulamak için aşağıdaki 6 haneli kodu girin.
              </p>
              <div style='background:#fdf0f9;border:2px dashed #f953c6;border-radius:10px;padding:24px;text-align:center;margin-bottom:32px;'>
                <p style='margin:0 0 6px;color:#888;font-size:12px;text-transform:uppercase;letter-spacing:1px;'><strong>Doğrulama Kodunuz</strong></p>
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
                    mail.To.Add(user.Email);
                    await smtp.SendMailAsync(mail);
                }
                catch
                {
                    ModelState.AddModelError(string.Empty, "Mail gönderilemedi. Lütfen tekrar deneyin.");
                    return View(loginUserDto);
                }

                ViewBag.ShowModal = true;
                return View(loginUserDto);
            }

            await _signInManager.SignInAsync(user, false);
            return RedirectToAction("Inbox", "Message");
        }

        [HttpPost]
        public async Task<IActionResult> VerifyLoginCode(string code)
        {
            var storedCode = HttpContext.Session.GetString("LoginVerifyCode");
            var userId = HttpContext.Session.GetString("LoginUserId");

            var user = await _userManager.FindByIdAsync(userId ?? "");

            if (storedCode == null || userId == null || storedCode != code)
            {
                ViewBag.ShowModal = true;
                ViewBag.VerifyError = "Girdiğiniz kod hatalı. Lütfen tekrar deneyin.";
                return View("UserLogin", new LoginUserDto
                {
                    Username = user?.UserName ?? ""
                });
            }

            user!.EmailConfirmed = true;
            await _userManager.UpdateAsync(user);

            HttpContext.Session.Clear();
            await _signInManager.SignInAsync(user, false);
            return RedirectToAction("Inbox", "Message");
        }
    }
}