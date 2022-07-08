
using BookShop.DataAccess.Data;
using BookShop.DataAccess.Repository.IRepository;
using BookShop.Models;
using BookShop.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookShopWebApp.Controllers
{
    [Area("Admin")]
    public class CompanyController : Controller
    {
        private readonly IUnitOFWork _context;

        public CompanyController(IUnitOFWork context , IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            IEnumerable<Company> company = _context.Company.GetAll();
            return View(company); 
        }

        public IActionResult Upsert(int? id)
        {
            var company = new Company();
            if (id==0 || id == null)
            {
                return View(company);
            }
            else
            {
                company = _context.Company.GetFirstOrDefault(c => c.Id == id);
                return View(company);
            }
            
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Company obj)
        {
           
            if (ModelState.IsValid)
            {
                if (obj.Id == 0)
                {
                    _context.Company.Add(obj);
                    TempData["Success"] = "Company Created Successfully.";
                }
                else
                {
                    _context.Company.Update(obj);
                    TempData["Success"] = "Company Updated Successfully.";
                }
                
                _context.Save();
                return RedirectToAction("index");
            }
            else
            {
                return View(obj);
            }
        }


        #region API CAll

        public IActionResult GetAll()
        {
            var companies = _context.Company.GetAll();

            return Ok( new {Data = companies });
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var company = _context.Company.GetFirstOrDefault(c => c.Id == id);
            if (company == null)
            {
                return Json(new {success = false, message="Error While Deleting!"});
            }
            _context.Company.Remove(company);
            _context.Save();
            return Json(new {success=true,message="Company Deleted Successfully!"});
        }

        #endregion
    }
}
