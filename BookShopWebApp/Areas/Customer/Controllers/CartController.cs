using System.Security.Claims;
using BookShop.DataAccess.Repository.IRepository;
using BookShop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookShopWebApp.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOFWork _unitOfWork;
        public ShoppingCartVm ShoppingCartVm { get; set; }
        public CartController(IUnitOFWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            ShoppingCartVm = new ShoppingCartVm
            {
                ListCart = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value,
                    includeProperties: "product") 
            };
            foreach (var cart in ShoppingCartVm.ListCart)
            {
                cart.Price = GetPriceBasedOnQuantity(cart.count,cart.product.Price, cart.product.Price50, cart.product.Price100);
                ShoppingCartVm.CartTotal += (cart.Price * cart.count);
            }
            return View(ShoppingCartVm);

        }
        public IActionResult Summary()
        {
            //var claimIdentity = (ClaimsIdentity)User.Identity;
            //var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            //ShoppingCartVm = new ShoppingCartVm
            //{
            //    ListCart = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value,
            //        includeProperties: "product") 
            //};
            //foreach (var cart in ShoppingCartVm.ListCart)
            //{
            //    cart.Price = GetPriceBasedOnQuantity(cart.count,cart.product.Price, cart.product.Price50, cart.product.Price100);
            //    ShoppingCartVm.CartTotal += (cart.Price * cart.count);
            //}
            return View();

        }

        private double GetPriceBasedOnQuantity(double count, double price, double price50, double price100)
        {
            if (count < 50)
            {
                return price;
            }
            else
            {
                if (count < 100)
                {
                    return price50;
                }
                return price100;
            }
        }

        public IActionResult Plus(int cartId)
        {
           var cart = _unitOfWork.ShoppingCart.GetFirstOrDefault(c => c.Id == cartId);
           _unitOfWork.ShoppingCart.IncrementCount(cart, 1);
           _unitOfWork.Save();
           return RedirectToAction(nameof(Index));
        }
        public IActionResult Minus(int cartId)
        {
           var cart = _unitOfWork.ShoppingCart.GetFirstOrDefault(c => c.Id == cartId);
           if (cart.count <= 1)
           {
               _unitOfWork.ShoppingCart.Remove(cart);
           }
           else
           {
               _unitOfWork.ShoppingCart.DecrementCount(cart, 1);
           }

           _unitOfWork.Save();
           return RedirectToAction(nameof(Index));
        }
        public IActionResult Remove(int cartId)
        {
           var cart = _unitOfWork.ShoppingCart.GetFirstOrDefault(c => c.Id == cartId);
           _unitOfWork.ShoppingCart.Remove(cart); 
           _unitOfWork.Save();
           return RedirectToAction(nameof(Index));
        }
    }
}
