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
        public async Task<IActionResult> Create(ProductTypes productTypes)
        {
            if (ModelState.IsValid)
            {
                this.db.Add(productTypes);
                await this.db.SaveChangesAsync();
                return this.RedirectToAction(nameof(Index));
            }
            return this.View(productTypes);
        }
    }
}