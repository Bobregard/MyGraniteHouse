using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyGraniteHouse.Data;
using MyGraniteHouse.Models;

namespace MyGraniteHouse.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductTypesController : Controller
    {
        private readonly ApplicationDbContext db;

        public ProductTypesController(ApplicationDbContext db)
        {
            this.db = db;
        }


        public IActionResult Index()
        {
            return View(this.db.ProductTypes.ToList());
        }

        public IActionResult Create()
        {
            return this.View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductTypes productType)
        {
            if (ModelState.IsValid)
            {
                this.db.Add(productType);
                await this.db.SaveChangesAsync();
                return this.RedirectToAction(nameof(Index));
            }
            return this.View(productType);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            var productType = await this.db.ProductTypes.FindAsync(id);

            if (productType == null)
            {
                return this.NotFound();
            }

            return this.View(productType);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductTypes productType)
        {
            if (id != productType.Id)
            {
                return this.NotFound();
            }

            if (ModelState.IsValid)
            {
                this.db.Update(productType);
                await this.db.SaveChangesAsync();
                return this.RedirectToAction(nameof(Index));
            }
            return this.View(productType);
        }
    }
}