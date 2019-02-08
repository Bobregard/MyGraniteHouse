using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyGraniteHouse.Data;
using MyGraniteHouse.Models;
using MyGraniteHouse.ViewModels;
using Microsoft.EntityFrameworkCore;
using MyGraniteHouse.Extensions;

namespace MyGraniteHouse.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class ShoppingCartController : Controller
    {
        private readonly ApplicationDbContext db;

        [BindProperty]
        public ShoppingCartViewModel ShoppingCartVM { get; set; }


        public ShoppingCartController(ApplicationDbContext db)
        {
            this.db = db;
            ShoppingCartVM = new ShoppingCartViewModel()
            {
                Products = new List<Product>()
            };


        }

        public async Task<IActionResult> Index()
        {
            List<int> listShoppingCart = HttpContext.Session.Get<List<int>>("ssShoppingCart");
            if (listShoppingCart.Count > 0)
            {
                foreach (var cartItem in listShoppingCart)
                {
                    Product prod = this.db.Products.Include(p => p.SpecialTags).Include(p => p.ProductTypes).Where(p => p.Id == cartItem).FirstOrDefault();
                    this.ShoppingCartVM.Products.Add(prod);
                }
            }

            return this.View(ShoppingCartVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Index")]
        public IActionResult IndexPost()
        {
            List<int> listCartItems = HttpContext.Session.Get<List<int>>("ssShoppingCart");

            ShoppingCartVM.Appointment.AppointmentDate
                .AddHours(ShoppingCartVM.Appointment.AppointmentTime.Hour)
                .AddMinutes(ShoppingCartVM.Appointment.AppointmentTime.Minute);

            Appointment appointment = ShoppingCartVM.Appointment;

            this.db.Appointments.Add(appointment);
            this.db.SaveChanges();

            int appointmentId = appointment.Id;

            foreach (var productId in listCartItems)
            {
                ProductsSelectedForAppointment productsSelectedForAppointment = new ProductsSelectedForAppointment()
                {
                    AppointmentId = appointmentId,
                    ProductId = productId
                };
                this.db.ProductsSelectedForAppointment.Add(productsSelectedForAppointment);               
            }
            this.db.SaveChanges();
            listCartItems = new List<int>();
            HttpContext.Session.Set("ssShoppingCart", listCartItems);

            return this.RedirectToAction("AppointmentConfirmation", "ShoppingCart", new { Id = appointmentId});
        }

        public IActionResult Remove(int id)
        {
            List<int> listCartItems = HttpContext.Session.Get<List<int>>("ssShoppingCart");

            if (listCartItems.Count > 0)
            {
                if (listCartItems.Contains(id))
                {
                    listCartItems.Remove(id);
                }
            }

            HttpContext.Session.Set("ssShoppingCart", listCartItems);
            return this.RedirectToAction(nameof(Index));
        }

        public IActionResult AppointmentConfirmation(int id)
        {
            this.ShoppingCartVM.Appointment = this.db.Appointments.Where(a => a.Id == id).FirstOrDefault();
            List<ProductsSelectedForAppointment> prodList = this.db.ProductsSelectedForAppointment.Where(p => p.AppointmentId == id).ToList();

            foreach (var prodAppointment in prodList)
            {
                this.ShoppingCartVM.Products.Add(this.db.Products.Include(p => p.ProductTypes)
                    .Include(p => p.SpecialTags)
                    .Where(p => p.Id == prodAppointment.ProductId)
                    .FirstOrDefault());
            }

            return this.View(ShoppingCartVM);
        }
    }
}