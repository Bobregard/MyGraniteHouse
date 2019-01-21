using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyGraniteHouse.Data;
using MyGraniteHouse.Models;
using MyGraniteHouse.Utility;
using MyGraniteHouse.ViewModels;

namespace MyGraniteHouse.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly IHostingEnvironment hostingEnvironment;

        [BindProperty]
        public ProductsViewModel productsVM { get; set; }

        public ProductsController(ApplicationDbContext db, IHostingEnvironment hostingEnvironment)
        {
            this.db = db;
            this.hostingEnvironment = hostingEnvironment;

            productsVM = new ProductsViewModel
            {
                ProductTypes = this.db.ProductTypes.ToList(),
                SpecialTags = this.db.SpecialTags.ToList(),
                Product = new Product()
            };
        }

        public async Task<IActionResult> Index()
        {
            var products = await this.db.Products.Include(m => m.ProductTypes).Include(m => m.SpecialTags).ToListAsync();

            return View(products);
        }

        public IActionResult Create()
        {
            return this.View(productsVM);
        }

        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePOST()
        {
            if (!ModelState.IsValid)
            {
                return this.View(productsVM);
            }

            this.db.Products.Add(productsVM.Product);
            await this.db.SaveChangesAsync();

            string webRoothPath = this.hostingEnvironment.WebRootPath;

            var files = HttpContext.Request.Form.Files;

            var productFromDb = this.db.Products.Find(productsVM.Product.Id);

            if (files.Count > 0)
            {
                var uploads = Path.Combine(webRoothPath, StaticDetails.ImageFolder);
                var extension = Path.GetExtension(files[0].FileName);

                using (var fileStream = new FileStream(Path.Combine(uploads, productsVM.Product.Id + extension), FileMode.Create))
                {
                    files[0].CopyTo(fileStream);
                }
                productFromDb.Image = @"\" + StaticDetails.ImageFolder + @"\" + productsVM.Product.Id + extension;
            }
            else
            {
                var uploads = Path.Combine(webRoothPath, StaticDetails.ImageFolder + @"\" + StaticDetails.DefaultProductImage);
                System.IO.File.Copy(uploads, webRoothPath + @"\" + StaticDetails.ImageFolder + @"\" + productsVM.Product.Id + ".png");
                productFromDb.Image = @"\" + StaticDetails.ImageFolder + @"\" + productsVM.Product.Id + ".png";
            }
            await this.db.SaveChangesAsync();

            return this.RedirectToAction(nameof(Index));
        }
    }
}