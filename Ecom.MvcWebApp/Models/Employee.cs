using System.ComponentModel.DataAnnotations;

namespace Ecom.MvcWebApp.Models;

public class Employee
{
    public int? Id {get; set;}
    
    [Required]
    public string? FirstName {get; set;}
    
    [Required]
    public string? LastName {get; set;}

    [Required]
    [EmailAddress]
    public string? Email {get; set;}

[   Required]
    public string? Address {get; set;}

}