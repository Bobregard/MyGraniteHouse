using MyGraniteHouse.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyGraniteHouse.ViewModels
{
    public class AppointmentDetailsViewModel
    {
        public Appointment Appointment { get; set; }

        public List<ApplicationUser> SalesPeople { get; set; }

        public List<Product> Products { get; set; }

    }
}
