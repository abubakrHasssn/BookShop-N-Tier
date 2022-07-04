
using BookShop.DataAccess.Data;
using BookShop.DataAccess.Repository.IRepository;
using BookShop.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookShopWebApp.Controllers
{
    public class ProductController : Controller
    {
        private readonly IUnitOFWork _context;

        public ProductController(IUnitOFWork context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            IEnumerable<Product> products = _context.Product.GetAll();
            return View(products); 
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Product product)
        {

            if (ModelState.IsValid)
            {
                _context.Product.Add(product);
                _context.Save();
                TempData["Success"] = "Product Created Successfully.";
                return RedirectToAction("index");
            }
            else
            {
                return View(product);
            }
        }

        public IActionResult Edit(int id)
        {
            var product = _context.Product.GetFirstOrDefault(c => c.Id == id);
            if(product == null)
                return View("Error");
            else
            {
                return View("Create",product);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(CoverType coverType)    
        {
            if (ModelState.IsValid)
            {
                _context.CoverType.Update(coverType);
                _context.Save();
                TempData["Success"] = "Cover Type Updated Successfully.";
                return RedirectToAction("Index");
            }
            else
            {
                return View("Create", coverType);
            }
        }

        public IActionResult Delete(int id)
        {
            var coverType = _context.CoverType.GetFirstOrDefault(c => c.Id == id);
            if (coverType == null)
            {
                return RedirectToAction("Index");
            }

            _context.CoverType.Remove(coverType);
            _context.Save();
            TempData["Success"] = "Cover Type Deleted Successfully.";
            return RedirectToAction("Index");
        }
    }
}
