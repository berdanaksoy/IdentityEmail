using IdentityEmail.Dtos;
using IdentityEmail.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityEmail.Controllers
{
    public class LoginController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager;
        public LoginController(SignInManager<AppUser> signInManager)
        {
            _signInManager = signInManager;
        }

        public IActionResult UserLogin()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UserLogin(LoginUserDto loginUserDto)
        {
            var result=await _signInManager.PasswordSignInAsync(loginUserDto.Username, loginUserDto.Password, false, false);
            if (result.Succeeded)
            {
                return RedirectToAction("Profile", "UserProfile");
            }
            return View();
        }
    }
}
