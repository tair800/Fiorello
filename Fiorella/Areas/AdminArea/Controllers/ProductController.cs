﻿using Fiorella.Areas.AdminArea.ViewModels.Product;
using Fiorella.Data;
using Fiorella.Extensions;
using Fiorella.Helpers;
using Fiorella.Models;
using Fiorella.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Fiorella.Areas.AdminArea.Controllers
{
    [Area("AdminArea")]

    public class ProductController : Controller
    {
        private readonly FiorelloDbContext _context;
        private readonly IEmailService _emailService;


        public ProductController(FiorelloDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }


        public async Task<IActionResult> Index()
        {
            var products = await _context.Products
                .Include(p => p.ProductImages)
                .Include(p => p.Category)
                .AsNoTracking()
                .ToListAsync();
            return View(products);
        }
        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name");
            return View();
        }



        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Create(ProductCreateVM productCreateVM)
        {
            ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name");
            if (!ModelState.IsValid) return View();
            var files = productCreateVM.Photos;
            Product newProduct = new();
            List<ProductImage> list = new();
            foreach (var file in files)
            {
                if (file.Length == 0)
                {
                    ModelState.AddModelError("Photos", "Can't be empty!");
                    return View(productCreateVM);
                }
                if (!file.CheckContentType())
                {
                    ModelState.AddModelError("Photos", "Choose right type!");
                    return View(productCreateVM);
                }
                if (file.CheckContentSize(500))
                {
                    ModelState.AddModelError("Photos", "Choose right Size!");
                    return View(productCreateVM);
                }
                ProductImage productImage = new();
                productImage.ImageUrl = await file.SaveFile();
                productImage.ProductId = newProduct.Id;
                if (files[0] == file)
                {
                    productImage.IsMain = true;
                }
                list.Add(productImage);
            }
            newProduct.ProductImages = list;
            newProduct.Name = productCreateVM.Name;
            newProduct.CategoryId = productCreateVM.CategoryId;
            newProduct.Price = productCreateVM.Price;
            newProduct.Count = productCreateVM.Count;

            await _context.Products.AddAsync(newProduct);
            await _context.SaveChangesAsync();



            List<string> emails = new() { "fiorello088@gmail.com", "tahir.aslanlee@gmail.com" };

            string body = $"<a href='http://localhost:5225/product/detail/{newProduct.Id}'>Go to product</a>";

            _emailService.SendEmail(emails, body, "New Product", "View product");





            return RedirectToAction(nameof(Index));
        }



        public async Task<IActionResult> Update(int? id)
        {
            ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name");


            if (id == null) return BadRequest();
            var product = await _context.Products.Include(p => p.ProductImages).Include(p => p.Category).FirstOrDefaultAsync(p => p.Id == id);
            if (product is null) return NotFound();

            return View(new ProductUpdateVM
            {
                Name = product.Name,
                Price = product.Price,
                Count = product.Count,
                ProductImages = product.ProductImages,
            });
        }



        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Update(int? id, ProductUpdateVM productUpdateVM)
        {
            ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name");

            if (id == null) return BadRequest();
            var product = await _context.Products
                .Include(p => p.ProductImages)
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product is null) return NotFound();
            if (!ModelState.IsValid) return View(productUpdateVM);


            ProductImage productImage = new();
            List<ProductImage> list = new();

            var files = productUpdateVM.Photos;
            productUpdateVM.ProductImages = product.ProductImages;
            if (files is not null)
            {
                if (files.Length > 4)
                {
                    ModelState.AddModelError("Photos", "Minimum 4 Photos!");
                    return View(productUpdateVM);
                }
                foreach (var file in files)
                {
                    if (!file.CheckContentType())
                    {
                        ModelState.AddModelError("Photos", "Choose right type!");
                        return View(productUpdateVM);
                    }
                    if (file.CheckContentSize(500))
                    {
                        ModelState.AddModelError("Photos", "Choose right Size!");
                        return View(productUpdateVM);
                    }

                    productImage.ImageUrl = await file.SaveFile();
                    productImage.ProductId = product.Id;
                    if (files[0] == file)
                    {
                        productImage.IsMain = true;
                    }
                    list.Add(productImage);
                }
                list.FirstOrDefault().IsMain = true;
                product.ProductImages = list;
            }
            product.Name = productUpdateVM.Name;
            product.Price = productUpdateVM.Price;
            product.CategoryId = productUpdateVM.CategoryId;
            product.Count = productUpdateVM.Count;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null) return BadRequest();
            var details = await _context.Products
                .AsNoTracking()
                .Include(l => l.ProductImages)
                .Include(l => l.Category)
                .FirstOrDefaultAsync(k => k.Id == id);
            if (details is null) return BadRequest();
            return View(details);
        }
        public async Task<IActionResult> SetMainPhoto(int? id)
        {
            if (id is null) return BadRequest();
            var image = await _context.ProductImages.FirstOrDefaultAsync(k => k.Id == id);
            if (image is null) return NotFound();
            image.IsMain = true;
            var mainImage = await _context.ProductImages.FirstOrDefaultAsync(k => k.IsMain && k.ProductId == image.ProductId);
            if (mainImage is null) return NotFound();
            mainImage.IsMain = false;
            await _context.SaveChangesAsync();
            return RedirectToAction("Detail", new { id = image.ProductId });
        }
        public async Task<IActionResult> DeleteImage(int? id)
        {
            if (id is null) return BadRequest();
            var image = await _context.ProductImages.FirstOrDefaultAsync(k => k.Id == id);
            if (image is null) return NotFound();
            Helper.DeleteImage(image.ImageUrl);
            _context.ProductImages.Remove(image);
            await _context.SaveChangesAsync();
            return RedirectToAction("Update", new { id = image.ProductId });
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null) return BadRequest();
            var product = await _context.Products.AsNoTracking().FirstOrDefaultAsync(k => k.Id == id);
            if (product is null) return NotFound();
            foreach (var image in product.ProductImages)
            {
                Helper.DeleteImage(image.ImageUrl);

            }
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

    }
}
