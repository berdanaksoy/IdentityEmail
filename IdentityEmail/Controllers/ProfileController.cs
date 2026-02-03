using IdentityEmail.Dtos;
using IdentityEmail.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace IdentityEmail.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly UserManager<AppUser> _userManager;

        public ProfileController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> UserProfile()
        {
            var values = await _userManager.FindByNameAsync(User.Identity.Name);
            UserEditDto userEditDto = new UserEditDto
            {
                Name = values.Name,
                Surname = values.Surname,
                Email = values.Email,
                ImageUrl = values.ImageUrl,
            };

            return View(userEditDto);
        }

        [HttpPost]
        public async Task<IActionResult> UserProfile(UserEditDto userEditDto)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            user.Name = userEditDto.Name;
            user.Surname = userEditDto.Surname;
            user.Email = userEditDto.Email;

            var resource = Directory.GetCurrentDirectory();
            var extension= Path.GetExtension(userEditDto.Image.FileName);
            var imageName= Guid.NewGuid()+extension;
            var saveLocation= resource+ "/wwwroot/images/" + imageName;
            var stream = new FileStream(saveLocation, FileMode.Create);
            await userEditDto.Image.CopyToAsync(stream);
            user.ImageUrl = imageName;

            user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, userEditDto.Password);
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return RedirectToAction("UserLogin", "Login");
            }
            return View();
        }
    }
}
