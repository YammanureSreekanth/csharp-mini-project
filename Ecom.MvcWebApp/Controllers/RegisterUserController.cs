using Ecom.MvcWebApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace Ecom.MvcWebApp.Controllers;

public class RegisterUserController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public IActionResult SaveUser()
    {
        return View();
    }

    [HttpPost]
    public IActionResult SaveUser(RegisterUser registerUser)
    {
        if (ModelState.IsValid)
        {
            TempData["Success"] = $"You are registered successfully {registerUser.FirstName} {registerUser.LastName}";
            return RedirectToAction(nameof(ThankYou));
        }
        return View(registerUser);
    }

    [HttpGet]
    public IActionResult ThankYou()
    {
        return View();
    }
}