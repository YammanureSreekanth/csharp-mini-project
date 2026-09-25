using Ecom.MvcWebApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace Ecom.MvcWebApp.Controllers;

public class LinqController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult ShowLinq()
    {
        List<User> users = new List<User>
        {
            new User { Id = "1", Name = "Sreekanth", Age = 35, Department = "IT", Gender = "Male", IsOnline = true },
            new User { Id = "2", Name = "Venkatesh", Age = 40, Department = "IT", Gender = "Male", IsOnline = true },
            new User { Id = "3", Name = "Aravind", Age = 34, Department = "Finace", Gender = "Male", IsOnline = true },
            new User { Id = "4", Name = "Mahesh", Age = 35, Department = "CustomerService", Gender = "Female", IsOnline = true },
            new User { Id = "5", Name = "Indu", Age = 35, Department = "IT", Gender = "Female", IsOnline = false },
            new User { Id = "6", Name = "Mahima", Age = 21, Department = "HR", Gender = "Female", IsOnline = true },
            new User { Id = "7", Name = "Naresh", Age = 29, Department = "Ops", Gender = "Male", IsOnline = false },
            new User { Id = "8", Name = "Madan", Age = 33, Department = "Ops", Gender = "Female", IsOnline = true },
            new User { Id = "9", Name = "Sreekanth", Age = 28, Department = "IT", Gender = "Male", IsOnline = false },
            new User { Id = "10", Name = "Kranthi", Age = 29, Department = "Logistics", Gender = "Male", IsOnline = true }
        };

        IEnumerable<User>? query = users.Where(e => e.Gender == "Male");
        List<User>? filterUsers = new List<User>();
        foreach (User user in query)
        {
            filterUsers.Add(user);
        }
        ViewBag.filterUsers = filterUsers;
        
        var selectQuery = users.Select(e => new
        {
            e.Name,
            e.Gender    
        });

        ViewBag.selectQuery = selectQuery;

        var selectReport = users.Select(e => new
        {
            e.Name,
            IsFromIT = e.Department == "IT"    
        });

        ViewBag.selectReport = selectReport;

        var orderByQuery = users.OrderBy(e => e.Name);

        ViewBag.orderByQuery = orderByQuery;

        var thenQuery = users
                                                    .OrderByDescending(e => e.Name)
                                                    .ThenBy(e => e.Department);

        ViewBag.orderByQuery = orderByQuery;

        return View();
    }
}