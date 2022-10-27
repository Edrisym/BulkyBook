using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApp.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAppMod.Models.ViewModels;
using WebApp.Utility;
using System.Security.Claims;
using WebAppMod.Models;
using Stripe.Checkout;
using Microsoft.Extensions.Options;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApp.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]

    public class CartController : Controller
    {

        private readonly IUnitOfWork _unitOfWork;
        [BindProperty]
        public ShoppingCardViewModel shoppingCardViewModel { get; set; }

        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            shoppingCardViewModel = new ShoppingCardViewModel()
            {
                ListCard = _unitOfWork.ShoppingCard.GetAll(u => u.ApplicationUserId == claim.Value, includeProperties: "Product"),
                OrderHeader = new()

            };

            foreach (var card in shoppingCardViewModel.ListCard)
            {
                card.Price = GetPriceBasedOnQuantity(card.Count, card.Product.Price, card.Product.Price50, card.Product.Price100);
                shoppingCardViewModel.OrderHeader.OrderTotal += (card.Price * card.Count);
            }

            return View(shoppingCardViewModel);
        }




        private double GetPriceBasedOnQuantity(double quantity, double price, double price50, double price100)
        {
            if (quantity <= 50)
            {
                return price;
            }
            else
            {
                if (quantity <= 100)
                {
                    return price50;
                }
                return price100;
            }
        }

        public IActionResult Summary()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            shoppingCardViewModel = new ShoppingCardViewModel()
            {
                ListCard = _unitOfWork.ShoppingCard.GetAll(u => u.ApplicationUserId == claim.Value, includeProperties: "Product"),
                OrderHeader = new()
            };

            shoppingCardViewModel.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(
                u => u.Id == claim.Value);

            shoppingCardViewModel.OrderHeader.Name = shoppingCardViewModel.OrderHeader.ApplicationUser.Name;
            shoppingCardViewModel.OrderHeader.PhoneNumber = shoppingCardViewModel.OrderHeader.ApplicationUser.PhoneNumber;
            shoppingCardViewModel.OrderHeader.StreetAddress = shoppingCardViewModel.OrderHeader.ApplicationUser.StreetAddress;
            shoppingCardViewModel.OrderHeader.City = shoppingCardViewModel.OrderHeader.ApplicationUser.City;
            shoppingCardViewModel.OrderHeader.State = shoppingCardViewModel.OrderHeader.ApplicationUser.State;
            shoppingCardViewModel.OrderHeader.PostalCode = shoppingCardViewModel.OrderHeader.ApplicationUser.PostalCode;

            foreach (var card in shoppingCardViewModel.ListCard)
            {
                card.Price = GetPriceBasedOnQuantity(card.Count, card.Product.Price, card.Product.Price50, card.Product.Price100);
                shoppingCardViewModel.OrderHeader.OrderTotal += (card.Price * card.Count);
            }

            return View(shoppingCardViewModel);
        }


        [ActionName("Summary")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SummaryPost()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            shoppingCardViewModel.ListCard = _unitOfWork.ShoppingCard.GetAll(u => u.ApplicationUserId == claim.Value, includeProperties: "Product");


            shoppingCardViewModel.OrderHeader.OrderDate = System.DateTime.Now;
            shoppingCardViewModel.OrderHeader.ApplicationUserId = claim.Value;

            foreach (var card in shoppingCardViewModel.ListCard)
            {
                card.Price = GetPriceBasedOnQuantity(card.Count, card.Product.Price, card.Product.Price50, card.Product.Price100);
                shoppingCardViewModel.OrderHeader.OrderTotal += (card.Price * card.Count);
            }

            ApplicationUser applicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == claim.Value);

            if (applicationUser.CompanyId.GetValueOrDefault() == 0)
            {
                shoppingCardViewModel.OrderHeader.PaymentStatus = StaticDetails.PaymentStatusPending;
                shoppingCardViewModel.OrderHeader.OrderStatus = StaticDetails.StatusPending;
            }
            else
            {
                shoppingCardViewModel.OrderHeader.PaymentStatus = StaticDetails.PaymentStatusDelayedPayment;

                shoppingCardViewModel.OrderHeader.OrderStatus = StaticDetails.StatusApproved;
            }

            _unitOfWork.OrderHeader.Add(shoppingCardViewModel.OrderHeader);
            _unitOfWork.Save();

            foreach (var card in shoppingCardViewModel.ListCard)
            {
                OrderDetail orderDetail = new()
                {
                    ProductId = card.ProductId,
                    OrderId = shoppingCardViewModel.OrderHeader.Id,
                    Price = card.Price,
                    Count = card.Count
                };

                _unitOfWork.OrderDetail.Add(orderDetail);
                _unitOfWork.Save();
            }



            if (applicationUser.CompanyId.GetValueOrDefault() == 0)
            {

                //stripe settings

                var domain = "http://localhost:26551/";
                var options = new SessionCreateOptions
                {
                    PaymentMethodTypes = new List<string>
                 {
                   "card",
                 },
                    LineItems = new List<SessionLineItemOptions>(),
                    Mode = "payment",
                    SuccessUrl = domain + $"Customer/Cart/OrderConfirmation?id={shoppingCardViewModel.OrderHeader.Id}",
                    CancelUrl = domain + $"Customer/Cart/Index",
                };

                foreach (var item in shoppingCardViewModel.ListCard)
                {

                    var sessionLineItem = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(item.Price * 100),//20.00 -> 2000
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.Product.Title
                            },

                        },
                        Quantity = item.Count,
                    };
                    options.LineItems.Add(sessionLineItem);

                }

                var service = new SessionService();
                Session session = service.Create(options);

                _unitOfWork.OrderHeader.UpdateStripePaymentId(shoppingCardViewModel.OrderHeader.Id, session.Id, session.PaymentIntentId);

                _unitOfWork.Save();
                Response.Headers.Add("Location", session.Url);

                return new StatusCodeResult(303);
            }

            else
            {
                return RedirectToAction("OrderConfirmation", "Cart", new
                {
                    id = shoppingCardViewModel.OrderHeader.Id
                });
            }
        }

        public IActionResult OrderConfirmation(int id)
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == id);
            if (orderHeader.PaymentStatus != StaticDetails.PaymentStatusDelayedPayment)
            {
                var service = new SessionService();
                Session session = service.Get(orderHeader.SessionId);
                //check the stripe status
                if (session.PaymentStatus.ToLower() == "paid")
                {
                    _unitOfWork.OrderHeader.UpdateStatus(id, StaticDetails.StatusApproved, StaticDetails.PaymentStatusApproved);
                    _unitOfWork.Save();
                }
            }

            List<ShoppingCard> shoppingCards = _unitOfWork.ShoppingCard.GetAll(u => u.ApplicationUserId == orderHeader.ApplicationUserId).ToList();

            _unitOfWork.ShoppingCard.RemoveRange(shoppingCards);
            _unitOfWork.Save();

            return View(id);
        }


        public IActionResult Plus(int cardId)
        {
            var card = _unitOfWork.ShoppingCard.GetFirstOrDefault(u => u.Id == cardId);
            _unitOfWork.ShoppingCard.IncrementCount(card, 1);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }


        public IActionResult Minus(int cardId)
        {
            var card = _unitOfWork.ShoppingCard.GetFirstOrDefault(u => u.Id == cardId);

            if (card.Count <= 1)
            {
                _unitOfWork.ShoppingCard.Remove(card);
            }
            else
            {
                _unitOfWork.ShoppingCard.DecrementCount(card, 1);
            }
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Remove(int cardId)
        {
            var card = _unitOfWork.ShoppingCard.GetFirstOrDefault(u => u.Id == cardId);
            _unitOfWork.ShoppingCard.Remove(card);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

    }
}

