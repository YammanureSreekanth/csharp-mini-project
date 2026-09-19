
using Ecom.MvcWebApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace Ecom.MvcWebApp.Controllers;

public class DataPassingController: Controller
{
    public IActionResult Index()
    {
        List<string> names = new List<string>();
        names.Add(item: "Aravind");
        names.Add(item: "Venkatesh");
        names.Add("Raj");

        ViewData["FirstName"] = "Sreekanth";
        ViewBag.LastName = "Yammanure";

        ViewBag.Names = names;
        ViewData["Names"] = names;

        return View();
    }

    [HttpGet]
    public IActionResult SaveEmployee()
    {
        return View();
    }

    [HttpPost]
    public IActionResult SaveEmployee(Employee employee)
    {
        TempData["Success"] = $"Thanks for submitting the form {employee.FirstName} {employee.LastName}";

        return RedirectToAction(nameof(ThankYou));
    }

    [HttpGet]
    public IActionResult ThankYou()
    {
        return View();
    }
}