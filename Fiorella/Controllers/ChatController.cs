using Fiorella.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Fiorella.Controllers
{
    public class ChatController : Controller
    {
        private readonly UserManager<AppUser> _userManager;

        public ChatController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public IActionResult Chat()
        {
            var existUser = User.Identity.Name;
            ViewBag.ExistUser = existUser;

            ViewBag.Users = _userManager.Users.ToList();
            return View();
        }
    }
}
