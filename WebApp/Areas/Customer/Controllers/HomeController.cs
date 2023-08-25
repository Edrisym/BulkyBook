using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WebAppMod.Models;
using Microsoft.Extensions.Logging;
using WebApp.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace WebApp.Controllers;
[Area("Customer")]

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IUnitOfWork _unitOfWork;


    public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public IActionResult Index()
    {
        IEnumerable<Product> productList = _unitOfWork.Product.GetAll(includeProperties: "Category,CoverType");
        return View(productList);
    }

    public IActionResult Details(int productId)
    {
        ShoppingCard cardObject = new()
        {
            Count = 1,
            ProductId = productId,
            Product = _unitOfWork.Product.GetFirstOrDefault(u => u.Id == productId, includeProperties: "Category,CoverType"),
        };


        return View(cardObject);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]

    public IActionResult Details(ShoppingCard shoppingCard)
    {
        var claimsIdentity = (ClaimsIdentity)User.Identity;
        var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
        shoppingCard.ApplicationUserId = claim.Value;

        ShoppingCard cardFromDb = _unitOfWork.ShoppingCard.GetFirstOrDefault(u => u.ApplicationUserId == claim.Value && u.ProductId == shoppingCard.ProductId);

        if (shoppingCard.Count <= 0)
        {
            TempData["error"] = "Can not be negative or zero";

            return RedirectToAction(nameof(Index));
        }

        if (cardFromDb == null)
        {
            _unitOfWork.ShoppingCard.Add(shoppingCard);
        }
        else
        {
            _unitOfWork.ShoppingCard.IncrementCount(cardFromDb, shoppingCard.Count);
        }

        _unitOfWork.Save();
        TempData["success"] = "Your Books have successfully added to your shop list!";

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

