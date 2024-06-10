using Fiorella.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Fiorella.Controllers
{
    public class BlogController : Controller
    {
        readonly FiorelloDbContext _context;

        public BlogController(FiorelloDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View(_context.Blogs.AsNoTracking().Take(3).OrderByDescending(b => b.Id).ToList());
        }
        public IActionResult Detail(int? id)
        {
            if (id is null) return BadRequest();
            var blog = _context.Blogs.AsNoTracking().FirstOrDefault(b => b.Id == id);
            if (blog == null) return NotFound();
            return View(blog);
        }
    }
}
