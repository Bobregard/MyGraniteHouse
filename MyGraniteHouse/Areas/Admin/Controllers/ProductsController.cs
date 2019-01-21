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

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            productsVM.Product = await this.db.Products.Include(m => m.ProductTypes).Include(m => m.SpecialTags).SingleOrDefaultAsync(m => m.Id == id);

            if (productsVM.Product == null)
            {
                return NotFound();
            }

            return this.View(productsVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id)
        {
            if (ModelState.IsValid)
            {
                string webRootPath = hostingEnvironment.WebRootPath;
                var files = HttpContext.Request.Form.Files;

                var productFromDb = this.db.Products.Where(m => m.Id == productsVM.Product.Id).FirstOrDefault();

                if (files.Count > 0 && files[0] != null)
                {
                    var uploads = Path.Combine(webRootPath, StaticDetails.ImageFolder);
                    var extensionNew = Path.GetExtension(files[0].FileName);
                    var extensionOld = Path.GetExtension(productFromDb.Image);

                    if (System.IO.File.Exists(Path.Combine(uploads, productsVM.Product.Id + extensionOld)))
                    {
                        System.IO.File.Delete(Path.Combine(uploads, productsVM.Product.Id + extensionOld));
                    }

                    using (var fileStream = new FileStream(Path.Combine(uploads, productsVM.Product.Id + extensionNew), FileMode.Create))
                    {
                        files[0].CopyTo(fileStream);
                    }
                    productsVM.Product.Image = @"\" + StaticDetails.ImageFolder + @"\" + productsVM.Product.Id + extensionNew;
                }

                if (productsVM.Product.Image != null)
                {
                    productFromDb.Image = productsVM.Product.Image;
                }

                productFromDb.Name = productsVM.Product.Name;
                productFromDb.Price = productsVM.Product.Price;
                productFromDb.Available = productsVM.Product.Available;
                productFromDb.ProductTypeId = productsVM.Product.ProductTypeId;
                productFromDb.SpecialTagsId = productsVM.Product.SpecialTagsId;
                productFromDb.ShadeColor = productsVM.Product.ShadeColor;

                await this.db.SaveChangesAsync();
                return this.RedirectToAction(nameof(Index));
            }

            return this.View(productsVM);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            productsVM.Product = await this.db.Products.Include(m => m.ProductTypes).Include(m => m.SpecialTags).SingleOrDefaultAsync(m => m.Id == id);

            if (productsVM.Product == null)
            {
                return NotFound();
            }

            return this.View(productsVM);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            productsVM.Product = await this.db.Products.Include(m => m.ProductTypes).Include(m => m.SpecialTags).SingleOrDefaultAsync(m => m.Id == id);

            if (productsVM.Product == null)
            {
                return NotFound();
            }

            return this.View(productsVM);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePost(int id)
        {
            string webRootPath = hostingEnvironment.WebRootPath;
            var product = await this.db.Products.FindAsync(id);

            if (product == null)
            {
                return this.NotFound();
            }
            else
            {
                var uploads = Path.Combine(webRootPath, StaticDetails.ImageFolder);
                var extension = Path.GetExtension(product.Image);

                if (System.IO.File.Exists(Path.Combine(uploads, product.Id + extension)))
                {
                    System.IO.File.Delete(Path.Combine(uploads, product.Id + extension));
                }

                this.db.Products.Remove(product);
                await this.db.SaveChangesAsync();

                return this.RedirectToAction(nameof(Index));
            }
        }
    }
}