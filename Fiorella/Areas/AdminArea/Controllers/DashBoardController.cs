using Microsoft.AspNetCore.Mvc;

namespace Fiorella.Areas.AdminArea.Controllers
{
    [Area("AdminArea")]
    public class DashBoardController : Controller
    {

        public IActionResult Index()
        {
            return View();
        }
    }
}
