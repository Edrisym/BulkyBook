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

public class CompanyController : Controller
{
    //dependancy injection
    private readonly IUnitOfWork _unitOfWork;
    //accessing the www root folder 

    public CompanyController(IUnitOfWork unitOfWork)
    {
        //passing the parameters

        _unitOfWork = unitOfWork;
    }

    public IActionResult Index()
    {
        return View();
    }



    //Get
    public IActionResult Upsert(int? id)
    {
        Company company = new();

        if (id == null || id == 0)
        {
            return View(company);
        }
        else
        {
            company = _unitOfWork.Company.GetFirstOrDefault(u => u.Id == id);
            return View(company);
            //Update
        }




    }

    //post 
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Upsert(Company obj)
    {

        if (ModelState.IsValid)
        {

            if (obj.Id == 0)
            {
                _unitOfWork.Company.Add(obj);
                TempData["success"] = "Company created successfully";

            }
            else
            {
                _unitOfWork.Company.Update(obj);
                TempData["success"] = "Company updated successfully";
            }
            _unitOfWork.Save();


            return RedirectToAction("Index");

        }
        return View(obj);
    }


    ////Get
    //public IActionResult Detail(int? id)
    //{
    //    if (id == null || id == 0)
    //    {
    //        return NotFound();
    //    }

    //    var companyFromDb = _unitOfWork.Company.GetFirstOrDefault(u => u.Id == id);

    //    if (companyFromDb == null)
    //    {
    //        return NotFound();
    //    }
    //    return View(companyFromDb);

    //}

    #region API CALLS
    [HttpGet]
    public IActionResult GetAll()
    {
        var companyList = _unitOfWork.Company.GetAll();
        return Json(new { Data = companyList });
    }
    //post
    [HttpDelete]
    public IActionResult Delete(int? id)
    {
        var companyObj = _unitOfWork.Company.GetFirstOrDefault(u => u.Id == id);
        if (companyObj == null)
        {
            return Json(new { success = false, message = "Error while deleting!" });
        }




        _unitOfWork.Company.Remove(companyObj);
        _unitOfWork.Save();
        return Json(new { success = true, message = "Delete Successful" });

    }
}
#endregion
