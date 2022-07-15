using System.Security.Claims;
using BookShop.DataAccess.Repository.IRepository;
using BookShop.Models;
using BookShop.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;

namespace BookShopWebApp.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOFWork _unitOfWork;

        [BindProperty]
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
                    includeProperties: "product"),
                OrderHeader = new()
        };
            foreach (var cart in ShoppingCartVm.ListCart)
            {
                cart.Price = GetPriceBasedOnQuantity(cart.count,cart.product.Price, cart.product.Price50, cart.product.Price100);
                ShoppingCartVm.OrderHeader.OrderTotal += (cart.Price * cart.count);
                
            }
            return View(ShoppingCartVm);

        }
        public IActionResult Summary()
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            ShoppingCartVm = new ShoppingCartVm
            {
                ListCart = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value,
                    includeProperties: "product"),
                OrderHeader = new()
            };
            ShoppingCartVm.OrderHeader.ApplicationUser =
                _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == claim.Value);

            ShoppingCartVm.OrderHeader.Name = ShoppingCartVm.OrderHeader.ApplicationUser.Name;
            ShoppingCartVm.OrderHeader.PhoneNumber = ShoppingCartVm.OrderHeader.ApplicationUser.PhoneNumber;
            ShoppingCartVm.OrderHeader.StreetAdderss = ShoppingCartVm.OrderHeader.ApplicationUser.StreetAddress;
            ShoppingCartVm.OrderHeader.City = ShoppingCartVm.OrderHeader.ApplicationUser.City;
            ShoppingCartVm.OrderHeader.State = ShoppingCartVm.OrderHeader.ApplicationUser.State;
            ShoppingCartVm.OrderHeader.PostalCode = ShoppingCartVm.OrderHeader.ApplicationUser.PostalCode;

            foreach (var cart in ShoppingCartVm.ListCart)
            {
                cart.Price = GetPriceBasedOnQuantity(cart.count, cart.product.Price, cart.product.Price50, cart.product.Price100);
                ShoppingCartVm.OrderHeader.OrderTotal += (cart.Price * cart.count);
            }
            return View(ShoppingCartVm);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Summary")]
        public IActionResult SummaryPOST()
        {
            //get user id
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            //Get cart list for the user
            ShoppingCartVm.ListCart = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value,
                includeProperties: "product");
            //adjust order status
            ShoppingCartVm.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
            ShoppingCartVm.OrderHeader.OrderStatus = SD.StatusPending;
            ShoppingCartVm.OrderHeader.OrderDate = System.DateTime.Now;
            ShoppingCartVm.OrderHeader.ApplicationUserId = claim.Value;

            foreach (var cart in ShoppingCartVm.ListCart)
            {
                cart.Price = GetPriceBasedOnQuantity(cart.count, cart.product.Price, cart.product.Price50, cart.product.Price100);
                ShoppingCartVm.OrderHeader.OrderTotal += (cart.Price * cart.count);
            }
            _unitOfWork.OrderHeader.Add(ShoppingCartVm.OrderHeader);
            _unitOfWork.Save();
            //Adding OrderDetails
            foreach (var cart in ShoppingCartVm.ListCart)
            {
                var OrderDetail = new OrderDetail
                {
                    ProductId = cart.ProductId,
                    OrderId = ShoppingCartVm.OrderHeader.Id,
                    Price = cart.Price,
                    Count = cart.count
                };
                _unitOfWork.OrderDetail.Add(OrderDetail);
                _unitOfWork.Save();
            }

            //StripeSetting
            var domain = "https://localhost:44301/";
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string>
                {
                    "card"
                },
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                SuccessUrl = domain + $"customer/cart/OrderConfirmation?id={ShoppingCartVm.OrderHeader.Id}",
                CancelUrl = domain + $"customer/cart/index",
            };


            foreach (var item in ShoppingCartVm.ListCart)
            {
                var sessionLineItem = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.Price * 100),
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.product.Title
                        }
                    },
                    Quantity = item.count,
                };
                options.LineItems.Add(sessionLineItem);
            }
            var service = new SessionService();
            Session session = service.Create(options);
            _unitOfWork.OrderHeader.UpdateStripePaymentId(ShoppingCartVm.OrderHeader.Id,session.Id,session.PaymentIntentId);
            _unitOfWork.Save();
            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);

            //_unitOfWork.ShoppingCart.RemoveRange(ShoppingCartVm.ListCart);
            //_unitOfWork.Save();
            //return RedirectToAction("Index", "Home");
        }

        public IActionResult OrderConfirmation(int id)
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == id);
            var service = new SessionService();
            Session session = service.Get(orderHeader.SessionId);
            //check the stripe status
            if (session.PaymentStatus.ToLower()=="paid")
            {
                _unitOfWork.OrderHeader.UpdateStatus(id,SD.StatusApproved,SD.PaymentStatusApproved);
                _unitOfWork.Save();
            }

            List<ShoppingCart> shoppingCarts = _unitOfWork.ShoppingCart
                .GetAll(u => u.ApplicationUserId == orderHeader.ApplicationUserId).ToList();

            _unitOfWork.ShoppingCart.RemoveRange(shoppingCarts);
            _unitOfWork.Save();
            return View(id);
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
