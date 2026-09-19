namespace Ecom.MvcWebApp.ViewComponents;

using Ecom.MvcWebApp.Models;
using Microsoft.AspNetCore.Mvc;

public class DynamicCategoryViewComponent: ViewComponent
{
    public IViewComponentResult Invoke(string Id, string Name, bool Online)
    {
        Catagory catagory = new Catagory();
        catagory.Id = Id;
        catagory.Name = Name;
        catagory.Online = Online;
        return View(catagory);
    }
}
