using Ecom.MvcWebApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace Ecom.MvcWebApp.Controllers;

public class CategoryController: Controller
{
    public IActionResult Index()
    {
        return Content("Sample Text");
    }

    public IActionResult Search()
    {
        Catagory catagory = new Catagory();
        catagory.Id = "suits";
        catagory.Name = "Suits";
        catagory.Online = true;

        return Json(catagory);
    }

    public IActionResult ShowView()
    {
        Catagory catagory = new Catagory();
        catagory.Id = "suits";
        catagory.Name = "Suits View";
        catagory.Online = true;

        return View(catagory);
    }
}