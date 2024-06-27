using Fiorella.Areas.AdminArea.ViewModels.Category;
using Fiorella.Data;
using Fiorella.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Fiorella.Areas.AdminArea.Controllers
{
    [Area("AdminArea")]
    public class CategoryController : Controller
    {
        private readonly FiorelloDbContext _context;

        public CategoryController(FiorelloDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _context.Categories
                .AsNoTracking()
                .ToListAsync();
            return View(categories);
        }
        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null) return BadRequest();
            var details = await _context.Categories.AsNoTracking().FirstOrDefaultAsync(k => k.Id == id);
            if (details is null) return BadRequest();
            return View(details);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Create(CategoryCreateVM category)
        {
            if (!ModelState.IsValid) return View(category);
            if (await _context.Categories.AnyAsync(c => c.Name.ToLower() == category.Name.ToLower()))
            {
                ModelState.AddModelError("Name", "Cant Create WIth Same Name");
                return View(category);
            }
            var newCategory = new Category()
            {
                Name = category.Name,
                Desc = category.Desc,
                CreatedDate = DateTime.Now

            };
            await _context.Categories.AddAsync(newCategory);
            await _context.SaveChangesAsync();
            return RedirectToAction("index");
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return BadRequest();
            var details = await _context.Categories.FirstOrDefaultAsync(k => k.Id == id);
            if (details is null) return BadRequest();
            _context.Categories.Remove(details);
            await _context.SaveChangesAsync();

            return RedirectToAction("index");
        }
    }
}
