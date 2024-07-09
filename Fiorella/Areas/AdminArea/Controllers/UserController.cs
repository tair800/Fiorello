using Fiorella.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Fiorella.Areas.AdminArea.Controllers
{
    [Area("adminarea")]
    public class UserController : Controller
    {
        private readonly UserManager<AppUser> _userManager;

        public UserController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(string searchText)
        {
            var users = string.IsNullOrWhiteSpace(searchText) ? await _userManager.Users.ToListAsync()
                : await _userManager.Users.Where(u => u.UserName.ToLower().Contains(searchText.ToLower()) ||
                u.FullName.ToLower().Contains(searchText.ToLower())).ToListAsync();

            return View(users);
        }
        public async Task<IActionResult> ChangeStatus(string id)
        {
            if (id is null) return BadRequest();
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();
            user.IsBlocked = !user.IsBlocked;
            await _userManager.UpdateAsync(user);
            return RedirectToAction("index");
        }
    }
}
