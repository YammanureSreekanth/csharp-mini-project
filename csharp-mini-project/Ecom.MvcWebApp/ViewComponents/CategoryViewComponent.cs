namespace Ecom.MvcWebApp.ViewComponents;

using Ecom.MvcWebApp.Models;
using Microsoft.AspNetCore.Mvc;

public class CategoryViewComponent: ViewComponent
{
    public IViewComponentResult Invoke()
    {
        Catagory catagory = new Catagory();
        catagory.Id = "suits";
        catagory.Name = "Suits";
        catagory.Online = true;
        return View(catagory);
    }
}
