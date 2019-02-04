﻿using MyGraniteHouse.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyGraniteHouse.ViewModels
{
    public class ShoppingCartViewModel
    {
        public List<Product> Products { get; set; }

        public Appointment Appointment { get; set; }

    }
}
