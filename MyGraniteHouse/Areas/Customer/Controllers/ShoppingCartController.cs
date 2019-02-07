using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyGraniteHouse.Data;
using MyGraniteHouse.Extensions;
using MyGraniteHouse.Models;
using MyGraniteHouse.ViewModels;
using Microsoft.EntityFrameworkCore;

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
                    ShoppingCartVM.Products.Add(prod);
                }
            }

            return View(ShoppingCartVM);
        }
    }
}