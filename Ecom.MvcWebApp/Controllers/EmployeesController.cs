using Ecom.MvcWebApp.Data;
using Ecom.MvcWebApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace Ecom.MvcWebApp.Controllers;

public class EmployeesController : Controller
{
    private readonly ApplicationDbContext _dbContext;
    public EmployeesController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public IActionResult Index()
    {
        List<Employee>? employees = _dbContext.Employees.ToList();
        return View(employees);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Create(Employee employee)
    {
        if (ModelState.IsValid)
        {
            _dbContext.Employees.Add(employee);
            _dbContext.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        return View(employee);
    }

    [HttpGet]
    public IActionResult Edit(int Id)
    {
        Employee? employee = _dbContext.Employees.Find(Id);
        return View(employee);
    }

    [HttpPost]
    public IActionResult Edit(Employee employee)
    {
        if (ModelState.IsValid)
        {
            _dbContext.Employees.Update(employee);
            _dbContext.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        return View(employee);
    }

    [HttpGet]
    public IActionResult Delete(int Id)
    {
        Employee? employee = _dbContext.Employees.Where(e => e.Id == Id).FirstOrDefault();
        return View(employee);
    }

    [HttpPost]
    [ActionName("Delete")]
    public IActionResult DeletePost(int Id)
    {
        Employee? employee = _dbContext.Employees.Find(Id);
        _dbContext.Employees.Remove(employee);
        _dbContext.SaveChanges();
        return RedirectToAction(nameof(Index));
    }
}