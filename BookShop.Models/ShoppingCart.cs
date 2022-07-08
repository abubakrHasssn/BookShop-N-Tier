using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookShop.Models
{
    public class ShoppingCart
    {
        public Product product { get; set; }

        public int count { get; set; }
    }
}
