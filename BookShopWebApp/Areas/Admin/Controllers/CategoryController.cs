
using BookShop.DataAccess.Data;
using BookShop.DataAccess.Repository.IRepository;
using BookShop.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookShopWebApp.Controllers
{
    public class CategoryController : Controller
    {
        private readonly IUnitOFWork _context;

        public CategoryController(IUnitOFWork context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            IEnumerable<Category> categories = _context.Category.GetAll();
            return View(categories); 
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category category)
        {

            if (ModelState.IsValid)
            {
                if (category.Name == category.DisplyOrder.ToString())
                {
                    ModelState.AddModelError("DisplyOrder","Display Order Cant Be Same As Category Name.");
                    return View(category);
                }
                _context.Category.Add(category);
                _context.Save();
                TempData["Success"] = "Category Created Successfully.";
                return RedirectToAction("index");
            }
            else
            {
                return View(category);
            }
        }

        public IActionResult Edit(int id)
        {
            var category = _context.Category.GetFirstOrDefault(c => c.Id == id);
            if(category == null)
                return View("Error");
            else
            {
                return View("Create",category);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Category category)    
        {
            if (ModelState.IsValid)
            {
                if (category.Name == category.DisplyOrder.ToString())
                {
                    ModelState.AddModelError("DisplyOrder", "Display Order Cant Be Same As Category Name.");
                    return View("Create",category);
                }

                _context.Category.Update(category);
                _context.Save();
                TempData["Success"] = "Category Updated Successfully.";
                return RedirectToAction("Index");
            }
            else
            {
                return View("Create", category);
            }
        }

        public IActionResult Delete(int id)
        {
            var category = _context.Category.GetFirstOrDefault(c => c.Id == id);
            if (category == null)
            {
                return RedirectToAction("Index");
            }

            _context.Category.Remove(category);
            _context.Save();
            TempData["Success"] = "Category Deleted Successfully.";
            return RedirectToAction("Index");
        }
    }
}
