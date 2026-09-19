using System.ComponentModel.DataAnnotations;
using Ecom.MvcWebApp.CustomValidations;

namespace Ecom.MvcWebApp.Models;

public class RegisterUser
{
    [Required]
    [StringLength(50, MinimumLength = 3)]
    public string? FirstName {get; set;}

    [Required(ErrorMessage = "Please enter your lastname")]
    [StringLength(50, MinimumLength = 3)]
    public string? LastName {get; set;}

    [Required]
    [StringLength(100, MinimumLength = 5)]
    [EmailAddress]
    // [RegularExpression("a-zA-Z0-9")]
    public string? Email {set; get;}

    [Required]
    [NotFutureDate]
    public DateTime? DateOfBirth {set; get;}

    [Required]
    [Range(18, 100)]
    public int? Age {set; get;}

    [Required]
    [StringLength(100, MinimumLength = 5)]
    [DataType(DataType.Password)]
    public string? Password {set; get;}

    [Required]
    [StringLength(100, MinimumLength = 5)]
    [DataType(DataType.Password)]
    [Compare(nameof(Password))]
    public string? ConfirmPassword {set; get;}

}