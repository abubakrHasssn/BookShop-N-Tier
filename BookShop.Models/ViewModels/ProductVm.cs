using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace BookShop.Models.ViewModels
{
    public class ProductVm
    {
        public Product product { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> categoriesList { get; set; }

        [ValidateNever]
        public IEnumerable<SelectListItem> coverTypesList { get; set; }

    }
}
