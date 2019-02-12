using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyGraniteHouse.Data;
using MyGraniteHouse.Models;
using MyGraniteHouse.Utility;
using MyGraniteHouse.ViewModels;

namespace MyGraniteHouse.Areas.Admin.Controllers
{
    [Authorize(Roles = StaticDetails.AdminEndUser + "," + StaticDetails.SuperAdminEndUser)]
    [Area("Admin")]
    public class AppointmentsController : Controller
    {
        private readonly ApplicationDbContext db;

        public AppointmentsController(ApplicationDbContext db)
        {
            this.db = db;
        }

        public async Task<IActionResult> Index(string searchName = null, string searchEmail = null, string searchPhone = null, string searchDate = null)
        {
            ClaimsPrincipal currentUser = this.User;
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            AppointmentViewModel appointmentVM = new AppointmentViewModel()
            {
                Appointments = new List<Appointment>()
            };

            appointmentVM.Appointments = db.Appointments.Include(a => a.SalesPerson).ToList();
            if (User.IsInRole(StaticDetails.AdminEndUser))
            {
                appointmentVM.Appointments = appointmentVM.Appointments.Where(a => a.SalesPersonId == claim.Value).ToList();
            }


            if (searchName != null)
            {
                appointmentVM.Appointments = appointmentVM.Appointments.Where(a => a.CustomerName.ToLower().Contains(searchName.ToLower())).ToList();
            }
            if (searchEmail != null)
            {
                appointmentVM.Appointments = appointmentVM.Appointments.Where(a => a.CustomerEmail.ToLower().Contains(searchEmail.ToLower())).ToList();
            }
            if (searchPhone != null)
            {
                appointmentVM.Appointments = appointmentVM.Appointments.Where(a => a.CustomerPhoneNumber.ToLower().Contains(searchPhone.ToLower())).ToList();
            }
            if (searchDate != null)
            {
                try
                {
                    DateTime appointmentDate = Convert.ToDateTime(searchDate);
                    appointmentVM.Appointments = appointmentVM.Appointments.Where(a => a.AppointmentDate.ToShortDateString()
                                        .Equals(appointmentDate.ToShortDateString()))
                                        .ToList();
                }
                catch (Exception ex)
                {

                    throw;
                }
                
            }


            return View(appointmentVM);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            Appointment currentApp = this.db.Appointments.Include(a => a.SalesPerson).FirstOrDefault(a => a.Id == id);

            //Unclude only non-deleted users
            List<ApplicationUser> salesPeople = this.db.ApplicationUsers.Where(s => s.LockoutEnd == null).ToList();

            var productList = (IEnumerable<Product>)(from p in this.db.Products
                                                     join a in this.db.ProductsSelectedForAppointment
                                                     on p.Id equals a.ProductId
                                                     where a.AppointmentId == id
                                                     select p).Include("ProductTypes");           

            AppointmentDetailsViewModel appVM = new AppointmentDetailsViewModel()
            {
                Appointment = currentApp,
                SalesPeople = salesPeople,
                Products = productList.ToList()
            };

            return this.View(appVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AppointmentDetailsViewModel appVM)
        {
            if (ModelState.IsValid)
            {
                appVM.Appointment.AppointmentDate = appVM.Appointment.AppointmentDate
                        .AddHours(appVM.Appointment.AppointmentTime.Hour)
                        .AddMinutes(appVM.Appointment.AppointmentTime.Minute);

                var appointmentFromDb = this.db.Appointments.Where(a => a.Id == appVM.Appointment.Id).FirstOrDefault();

                appointmentFromDb.CustomerName = appVM.Appointment.CustomerName;
                appointmentFromDb.CustomerEmail = appVM.Appointment.CustomerEmail;
                appointmentFromDb.CustomerPhoneNumber = appVM.Appointment.CustomerPhoneNumber;
                appointmentFromDb.AppointmentDate = appVM.Appointment.AppointmentDate;
                appointmentFromDb.IsConfirmed = appVM.Appointment.IsConfirmed;

                if (User.IsInRole(StaticDetails.SuperAdminEndUser))
                {
                    appointmentFromDb.SalesPersonId = appVM.Appointment.SalesPersonId;
                }
                await this.db.SaveChangesAsync();

                return this.RedirectToAction(nameof(Index));
            }

            return this.View(appVM);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            Appointment currentApp = this.db.Appointments.Include(a => a.SalesPerson).FirstOrDefault(a => a.Id == id);

            //Unclude only non-deleted users
            List<ApplicationUser> salesPeople = this.db.ApplicationUsers.Where(s => s.LockoutEnd == null).ToList();

            var productList = (IEnumerable<Product>)(from p in this.db.Products
                                                     join a in this.db.ProductsSelectedForAppointment
                                                     on p.Id equals a.ProductId
                                                     where a.AppointmentId == id
                                                     select p).Include("ProductTypes");

            AppointmentDetailsViewModel appVM = new AppointmentDetailsViewModel()
            {
                Appointment = currentApp,
                SalesPeople = salesPeople,
                Products = productList.ToList()
            };

            return this.View(appVM);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            Appointment currentApp = this.db.Appointments.Include(a => a.SalesPerson).FirstOrDefault(a => a.Id == id);

            //Unclude only non-deleted users
            List<ApplicationUser> salesPeople = this.db.ApplicationUsers.Where(s => s.LockoutEnd == null).ToList();

            var productList = (IEnumerable<Product>)(from p in this.db.Products
                                                     join a in this.db.ProductsSelectedForAppointment
                                                     on p.Id equals a.ProductId
                                                     where a.AppointmentId == id
                                                     select p).Include("ProductTypes");

            AppointmentDetailsViewModel appVM = new AppointmentDetailsViewModel()
            {
                Appointment = currentApp,
                SalesPeople = salesPeople,
                Products = productList.ToList()
            };

            return this.View(appVM);
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePost(int id)
        {
            var appointment = await this.db.Appointments.FindAsync(id);

            this.db.Appointments.Remove(appointment);
            await this.db.SaveChangesAsync();

            return this.RedirectToAction(nameof(Index));
        }
    }
}