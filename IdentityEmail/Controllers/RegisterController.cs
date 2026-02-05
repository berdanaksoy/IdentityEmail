using IdentityEmail.Dtos;
using IdentityEmail.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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
        public IActionResult CreateUser()
        {
            return View();
        }

        Random rnd = new Random();
        int x = rnd.Next(100000,1000000);
        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserRegisterDto createUserRegisterDto)
        {
            AppUser appUser = new()
            {
                Name = createUserRegisterDto.Name,
                Surname = createUserRegisterDto.Surname,
                UserName = createUserRegisterDto.Username,
                Email = createUserRegisterDto.Email,
                ConfirmCode = x.ToString()
            };

            var result = await _userManager.CreateAsync(appUser, createUserRegisterDto.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("UserLogin", "Login");
            }
            else
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }
            }

            return View();
        }
    }
}
