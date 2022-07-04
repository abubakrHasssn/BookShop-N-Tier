
using BookShop.DataAccess.Data;
using BookShop.DataAccess.Repository.IRepository;
using BookShop.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookShopWebApp.Controllers
{
    public class CoverTypeController : Controller
    {
        private readonly IUnitOFWork _context;

        public CoverTypeController(IUnitOFWork context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            IEnumerable<CoverType> coverType = _context.CoverType.GetAll();
            return View(coverType); 
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CoverType coverType)
        {

            if (ModelState.IsValid)
            {
                _context.CoverType.Add(coverType);
                _context.Save();
                TempData["Success"] = "Cover Type Created Successfully.";
                return RedirectToAction("index");
            }
            else
            {
                return View(coverType);
            }
        }

        public IActionResult Edit(int id)
        {
            var coverType = _context.CoverType.GetFirstOrDefault(c => c.Id == id);
            if(coverType == null)
                return View("Error");
            else
            {
                return View("Create",coverType);
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
