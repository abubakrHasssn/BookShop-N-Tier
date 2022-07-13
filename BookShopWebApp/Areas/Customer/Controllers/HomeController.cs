using BookShop.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using BookShop.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;


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
            IEnumerable<Product> products = _context.Product.GetAll(includeProperties:"Category,CoverType");
            return View(products);
        }

        public IActionResult Details(int ProductId)
        {
            var CartObj = new ShoppingCart
            {
                product = _context.Product.GetFirstOrDefault(p => p.Id == ProductId, "Category,CoverType"),
                count = 1
            };
            return View(CartObj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Details(ShoppingCart shoppingCart)
        {
            var claimIdentity =(ClaimsIdentity) User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            shoppingCart.ApplicationUserId = claim.Value;

            ShoppingCart cartFromDb = _context.ShoppingCart.GetFirstOrDefault(u =>
                u.ApplicationUserId == claim.Value && u.ProductId == shoppingCart.ProductId);
            if (cartFromDb == null)
            {
                _context.ShoppingCart.Add(shoppingCart);
            }
            else
            {
                _context.ShoppingCart.IncrementCount(cartFromDb, shoppingCart.count);
            }
           
            _context.Save();
            return RedirectToAction(nameof(Index));
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