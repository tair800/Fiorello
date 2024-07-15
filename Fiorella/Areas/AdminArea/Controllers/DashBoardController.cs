using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fiorella.Areas.AdminArea.Controllers
{
    [Area("AdminArea")]
    [Authorize(Roles = "admin")]
    public class DashBoardController : Controller
    {

        public IActionResult Index()
        {
            return View();
        }
        //[AllowAnonymous]//bu kod qorunmur bele yazanda
        //public IActionResult Index1()
        //{
        //    return View();
        //}
    }
}
