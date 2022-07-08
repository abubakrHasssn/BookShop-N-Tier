using BookShop.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using BookShop.DataAccess.Repository.IRepository;


namespace BookShopWebApp.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOFWork _context ;

        public HomeController(ILogger<HomeController> logger , IUnitOFWork context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> products = _context.Product.GetAll("Category,CoverType");
            return View(products);
        }

        public IActionResult Details(int id)
        {
            var CartObj = new ShoppingCart
            {
                product = _context.Product.GetFirstOrDefault(p => p.Id == id, "Category,CoverType"),
                count = 1

            };
            return View(CartObj);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}