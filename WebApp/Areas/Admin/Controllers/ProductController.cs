using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebApp.DataAccess;
using WebAppMod.Models;
using WebApp.DataAccess.Repository.IRepository;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebAppMod.Models.ViewModels;
using System.IO;

namespace WebApp.Controllers;
[Area("Admin")]

public class ProductController : Controller
{
    //dependancy injection
    private readonly IUnitOfWork _unitOfWork;
    //accessing the www root folder 
    private readonly IWebHostEnvironment _webHostEnvironment;

    public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
    {
        //passing the parameters

        _unitOfWork = unitOfWork;
        _webHostEnvironment = webHostEnvironment;
    }

    public IActionResult Index()
    {
        return View();
    }



    //Get
    public IActionResult Upsert(int? id)
    {
        ProductViewModel productViewModelObj = new()
        {
            //Drop Downs
            Product = new(),
            CategoryList = _unitOfWork.Category.GetAll().Select(
              u => new SelectListItem
              {
                  Text = u.Name,
                  Value = u.Id.ToString()
              }),

            CoverTypeList = _unitOfWork.CoverType.GetAll().Select(
            u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString()
            }),
        };

        if (id == null || id == 0)
        {
            /*Create product
           
            ViewBag.CategoryList = CategoryList;
            be aware of differences between these 
            ViewData["CoverTypeList"] = CoverTypeList; */

            return View(productViewModelObj);
        }
        else
        {
            productViewModelObj.Product = _unitOfWork.Product.GetFirstOrDefault(u => u.Id == id);
            return View(productViewModelObj);
            //Update
        }




    }

    //post 
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Upsert(ProductViewModel objProductViewModel, IFormFile? file)
    {
        if (ModelState.IsValid)
        {
            string wwwRootPath = _webHostEnvironment.WebRootPath;
            if (file != null)
            {
                string fileName = Guid.NewGuid().ToString();
                var uploads = Path.Combine(wwwRootPath, @"Images/Products");
                var extension = Path.GetExtension(file.FileName);

                //Deleting the old image

                if (objProductViewModel.Product.ImageUrl != null)
                {
                    var oldImagePath = Path.Combine(wwwRootPath, objProductViewModel.Product.ImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                {
                    file.CopyTo(fileStreams);
                }
                objProductViewModel.Product.ImageUrl = @"/Images/Products/" + fileName + extension;
            }

            if (objProductViewModel.Product.Id == 0)
            {
                _unitOfWork.Product.Add(objProductViewModel.Product);
            }
            else
            {
                _unitOfWork.Product.Update(objProductViewModel.Product);
            }
            _unitOfWork.Save();
            TempData["success"] = "Product created successfully";
            return RedirectToAction("Index");
        }
        return View(objProductViewModel);
    }

    //Get
    public IActionResult Detail(int? id)
    {
        if (id == null || id == 0)
        {
            return NotFound();
        }

        var categoryFromDb = _unitOfWork.Category.GetFirstOrDefault(u => u.Id == id);

        if (categoryFromDb == null)
        {
            return NotFound();
        }
        return View(categoryFromDb);

    }

    #region API CALLS
    [HttpGet]
    public IActionResult GetAll()
    {
        var productList = _unitOfWork.Product.GetAll(includeProperties: "Category,CoverType");
        return Json(new { Data = productList });
    }
    //post
    [HttpDelete]
    public IActionResult Delete(int? id)
    {
        var obj = _unitOfWork.Product.GetFirstOrDefault(u => u.Id == id);
        if (obj == null)
        {
            return Json(new { success = false, message = "Error while deleting!" });
        }
        //else
        //{
        //    _unitOfWork.Product.Remove(obj);
        //    TempData["error"] = "Category Deleted successfully";
        //    return RedirectToAction("Index");

        //}

        var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, obj.ImageUrl.TrimStart('/'));
        if (System.IO.File.Exists(oldImagePath))
        {
            System.IO.File.Delete(oldImagePath);
        }


        _unitOfWork.Product.Remove(obj);
        _unitOfWork.Save();
        return Json(new { success = true, message = "Delete Successful" });

    }
}
#endregion
