using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApp.DataAccess;
using WebAppMod.Models;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using WebApp.DataAccess.Repository.IRepository;




namespace WebApp.Controllers;
[Area("Admin")]

public class CoverTypeController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public CoverTypeController(IUnitOfWork unitOfWrok)
    {
        _unitOfWork = unitOfWrok;
    }

    // GET: CategoryTemp
    //public Task<IActionResult> Index()
    public IActionResult Index()
    {

        //IEnumerable<Category> objCategoryList = _context.Categories;

        IEnumerable<CoverType> objCoverToList = _unitOfWork.CoverType.GetAll();
        return View(objCoverToList);
    }

    // GET: CategoryTemp/Details/5
    public IActionResult Details(int? id)
    {
        if (id == null || _unitOfWork.CoverType == null)
        {
            return NotFound();
        }

        var coverType = _unitOfWork.CoverType
            .GetFirstOrDefault(m => m.Id == id);
        if (coverType == null)
        {
            return NotFound();
        }

        return View(coverType);
    }

    // GET: CategoryTemp/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: CategoryTemp/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create([Bind("Id,Name,DisplayOrder,CreatedDateTime")] CoverType covertype)
    {
        if (ModelState.IsValid)
        {
            _unitOfWork.CoverType.Add(covertype);
            _unitOfWork.Save();
            TempData["success"] = "Covertype Created successfully";
            return RedirectToAction(nameof(Index));
        }
        return View(covertype);
    }

    // GET: CategoryTemp/Edit/5
    public IActionResult Edit(int? id)
    {
        if (id == null || _unitOfWork.CoverType == null)
        {
            return NotFound();
        }

        var coverTypeFromDbFirst = _unitOfWork.CoverType.GetFirstOrDefault(u => u.Id == id);
        if (coverTypeFromDbFirst == null)
        {
            return NotFound();
        }
        return View(coverTypeFromDbFirst);
    }

    // POST: CategoryTemp/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, [Bind("Id,Name,DisplayOrder,CreatedDateTime")] CoverType covertype)
    {
        if (id != covertype.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            _unitOfWork.CoverType.Update(covertype);
            _unitOfWork.Save();
            TempData["success"] = "coverType updated successfully";
        }
        return RedirectToAction(nameof(Index));
    }

    // GET: coverType/Delete/5
    public IActionResult Delete(int? id)
    {
        if (id == null || _unitOfWork.CoverType == null)
        {
            return NotFound();
        }

        var covertype = _unitOfWork.CoverType
            .GetFirstOrDefault(m => m.Id == id);
        if (covertype == null)
        {
            return NotFound();
        }

        return View(covertype);
    }

    // POST: coverType /Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        if (_unitOfWork.CoverType == null)
        {
            return Problem("Entity set 'ApplicationDbContext.Categories'  is null.");
        }
        var covertype = _unitOfWork.CoverType
                   .GetFirstOrDefault(m => m.Id == id);
        if (covertype != null)
        {
            _unitOfWork.CoverType.Remove(covertype);
        }

        _unitOfWork.Save();
        TempData["error"] = "CoverType Deleted successfully";
        return RedirectToAction(nameof(Index));
    }


}

