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

        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserRegisterDto createUserRegisterDto)
        {
            AppUser appUser = new()
            {
                Name = createUserRegisterDto.Name,
                Surname = createUserRegisterDto.Surname,
                UserName = createUserRegisterDto.Username,
                Email = createUserRegisterDto.Email
            };

            await _userManager.CreateAsync(appUser, createUserRegisterDto.Password);

            return RedirectToAction("UserList");
        }
    }
}
