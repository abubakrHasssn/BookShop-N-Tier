
using BookShop.DataAccess.Data;
using BookShop.DataAccess.Repository.IRepository;
using BookShop.Models;
using BookShop.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookShopWebApp.Controllers
{
    public class ProductController : Controller
    {
        private readonly IUnitOFWork _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IUnitOFWork context , IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            IEnumerable<Product> products = _context.Product.GetAll();
            return View(products); 
        }

        public IActionResult Upsert(int? id)
        {

            var product = new ProductVm
            {
                product = new Product(),
                categoriesList = _context.Category.GetAll().Select(c=> new SelectListItem( c.Name ,c.Id.ToString())),
                coverTypesList = _context.CoverType.GetAll().Select(c => new SelectListItem(c.Name, c.Id.ToString()))
            };

            if (id==0 || id == null)
            {
                return View(product);
            }
            else
            {
                product.product = _context.Product.GetFirstOrDefault(p => p.Id == id);
                return View(product);
            }
            
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductVm obj, IFormFile? file)
        {
           
            if (ModelState.IsValid)
            {
                string rootPath = _webHostEnvironment.WebRootPath;
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString();
                    var uploads= Path.Combine(rootPath, @"Images\products");
                    var extension = Path.GetExtension(file.FileName);
                    if (obj.product.ImageUrl != null)
                    {
                        var oldPath = Path.Combine(rootPath,obj.product.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldPath))
                        {
                            System.IO.File.Delete(oldPath);
                        }
                    }
                    using (var fileStreams = new FileStream(Path.Combine(uploads,fileName+extension),FileMode.Create))
                    {
                        file.CopyTo(fileStreams);
                    }

                    obj.product.ImageUrl = @"\Images\Products\" + fileName + extension;
                }

                if (obj.product.Id == 0)
                {
                    _context.Product.Add(obj.product);
                    TempData["Success"] = "Product Created Successfully.";
                }
                else
                {
                    _context.Product.Update(obj.product);
                    TempData["Success"] = "Product Updated Successfully.";
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
            var products = _context.Product.GetAll("Category");

            return Ok( new {Data = products});
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var product = _context.Product.GetFirstOrDefault(c => c.Id == id);
            if (product == null)
            {
                return Json(new {success = false, message="Error while deleting!"});
            }
            if (product.ImageUrl != null)
            {
                var oldPath = Path.Combine(_webHostEnvironment.WebRootPath, product.ImageUrl.TrimStart('\\'));
                if (System.IO.File.Exists(oldPath))
                {
                    System.IO.File.Delete(oldPath);
                }
            }

            _context.Product.Remove(product);
            _context.Save();
            return Json(new {success=true,message="Product Deleted Successfully!"});
        }

        #endregion
    }
}
