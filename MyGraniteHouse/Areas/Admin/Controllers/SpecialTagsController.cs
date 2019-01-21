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
    public class SpecialTagsController : Controller
    {
        private readonly ApplicationDbContext db;
        
        public SpecialTagsController(ApplicationDbContext db)
        {
            this.db = db;
        }


        public IActionResult Index()
        {
            return View(this.db.SpecialTags.ToList());
        }


        public IActionResult Create()
        {
            return this.View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SpecialTags specialTag)
        {
            if (ModelState.IsValid)
            {
                this.db.SpecialTags.Add(specialTag);
                await this.db.SaveChangesAsync();
                return this.RedirectToAction(nameof(Index));
            }
            return this.View(specialTag);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            var specialTag = await this.db.SpecialTags.FindAsync(id);

            if (specialTag == null)
            {
                return this.NotFound();
            }

            return this.View(specialTag);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SpecialTags specialTag)
        {
            if (id != specialTag.Id)
            {
                return this.NotFound();
            }

            if (ModelState.IsValid)
            {
                this.db.SpecialTags.Update(specialTag);
                await this.db.SaveChangesAsync();
                return this.RedirectToAction(nameof(Index));
            }

            return this.View(specialTag);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            var specialTag = await this.db.SpecialTags.FindAsync(id);

            if (specialTag == null)
            {
                return this.NotFound();
            }

            return this.View(specialTag);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            var specialTag = await this.db.SpecialTags.FindAsync(id);

            if (specialTag == null)
            {
                return this.NotFound();
            }

            return this.View(specialTag);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var specialTag = await this.db.SpecialTags.FindAsync(id);
            this.db.SpecialTags.Remove(specialTag);
            await this.db.SaveChangesAsync();
            return this.RedirectToAction(nameof(Index));

        }
    }
}