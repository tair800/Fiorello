using Fiorella.Data;
using Fiorella.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Fiorella.Controllers
{
    public class HomeController : Controller
    {
        private readonly FiorelloDbContext _context;

        public HomeController(FiorelloDbContext context)
        {
            _context = context;

        }

        public IActionResult Index()
        {


            var sliders = _context.Sliders.AsNoTracking().ToList();
            var sliderEntities = _context.SliderEntities.AsNoTracking().SingleOrDefault();
            var categories = _context.Categories.ToList();
            var products = _context.Products.Include(p => p.ProductImages).ToList();
            var homeVM = new HomeVM()
            {
                Sliders = sliders,
                SliderEntities = sliderEntities,
                Categories = categories,
                Products = products,

            };
            return View(homeVM);
        }


    }
}
