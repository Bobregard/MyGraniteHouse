using MyGraniteHouse.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyGraniteHouse.ViewModels
{
    public class ProductsViewModel
    {
        public Product Product { get; set; }

        public IEnumerable<ProductTypes> ProductTypes { get; set; }

        public IEnumerable<SpecialTags> SpecialTags { get; set; }
    }
}
