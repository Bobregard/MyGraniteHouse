using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyGraniteHouse.Data;
using MyGraniteHouse.Models;
using MyGraniteHouse.Utility;

namespace MyGraniteHouse.Areas.Admin.Controllers
{
    [Authorize(Roles = StaticDetails.SuperAdminEndUser)]
    [Area("Admin")]
    public class AdminUsersController : Controller
    {
        private readonly ApplicationDbContext db;

        public AdminUsersController(ApplicationDbContext db)
        {
            this.db = db;
        }

        public IActionResult Index()
        {
            return View(this.db.ApplicationUsers.ToList());
        }

        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || id.Trim().Length == 0)
            {
                return this.NotFound();
            }

            var user = await this.db.ApplicationUsers.FindAsync(id);

            if (user == null)
            {
                return this.NotFound();
            }

            return this.View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(string id, ApplicationUser user)
        {
            if (id != user.Id)
            {
                return this.NotFound();
            }
            if (ModelState.IsValid)
            {
                ApplicationUser userFromDb = this.db.ApplicationUsers.Where(u => u.Id == id).FirstOrDefault();
                userFromDb.Name = user.Name;
                userFromDb.PhoneNumber = user.PhoneNumber;

                this.db.SaveChanges();
                return this.RedirectToAction(nameof(Index));
            }

            return this.View(user);
        }

        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || id.Trim().Length == 0)
            {
                return this.NotFound();
            }

            var user = await this.db.ApplicationUsers.FindAsync(id);

            if (user == null)
            {
                return this.NotFound();
            }

            return this.View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public IActionResult DeletePost(string id)
        {

            ApplicationUser userFromDb = this.db.ApplicationUsers.Where(u => u.Id == id).FirstOrDefault();
            userFromDb.LockoutEnd = DateTime.Now.AddYears(1000);

            this.db.SaveChanges();
            return this.RedirectToAction(nameof(Index));

        }
    }
}