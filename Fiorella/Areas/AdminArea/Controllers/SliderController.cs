using Fiorella.Areas.AdminArea.ViewModels.Slider;
using Fiorella.Data;
using Fiorella.Extensions;
using Fiorella.Helpers;
using Fiorella.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Fiorella.Areas.AdminArea.Controllers
{
    [Area("AdminArea")]
    public class SliderController : Controller
    {

        private readonly FiorelloDbContext _context;

        public SliderController(FiorelloDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var slider = _context.Sliders.AsNoTracking().ToList();
            return View(slider);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Create(SliderCreateVM sliderCreateVM)
        {
            var file = sliderCreateVM.Photo;
            if (file == null)
            {
                ModelState.AddModelError("Photo", "Can't be empty!");
                return View(sliderCreateVM);
            }
            if (!file.CheckContentType())
            {
                ModelState.AddModelError("Photo", "Choose right type!");
                return View(sliderCreateVM);
            }
            if (file.CheckContentSize(500))
            {
                ModelState.AddModelError("Photo", "Choose right Size!");
                return View(sliderCreateVM);
            }

            Slider slider = new();
            slider.ImgUrl = await file.SaveFile();

            await _context.Sliders.AddAsync(slider);
            await _context.SaveChangesAsync();
            return RedirectToAction("index");
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null) return BadRequest();
            var slider = _context.Sliders.FirstOrDefault(s => s.Id == id);
            if (slider is null) return NotFound();

            Helper.DeleteImage(slider.ImgUrl);

            _context.Sliders.Remove(slider);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }
        public async Task<IActionResult> Update(int? id)
        {
            if (id is null) return BadRequest();
            var slider = _context.Sliders.FirstOrDefault(s => s.Id == id);
            if (slider is null) return NotFound();
            return View(new SliderUpdateVM { ImageUrl = slider.ImgUrl });
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Update(int? id, SliderUpdateVM sliderUpdateVM)
        {
            if (id is null) return BadRequest();
            var slider = _context.Sliders.FirstOrDefault(s => s.Id == id);
            if (slider is null) return NotFound();
            var file = sliderUpdateVM.Photo;
            if (file == null)
            {
                ModelState.AddModelError("Photo", "Can't be empty!");
                sliderUpdateVM.ImageUrl = slider.ImgUrl;
                return View(sliderUpdateVM);
            }
            if (!file.CheckContentType())
            {
                ModelState.AddModelError("Photo", "Choose right type!");
                return View(sliderUpdateVM);
            }
            if (file.CheckContentSize(500))
            {
                ModelState.AddModelError("Photo", "Choose right Size!");
                return View(sliderUpdateVM);
            }
            string fileName = await file.SaveFile();
            Helper.DeleteImage(slider.ImgUrl);
            slider.ImgUrl = fileName;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));


        }
    }
}
