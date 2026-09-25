using System.ComponentModel.DataAnnotations;

namespace Ecom.MvcWebApp.CustomValidations;

public class NotFutureDateAttribute: ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        // saving value to date if it's DateTime type
        if (value is DateTime date)
        {
            return date.Date <= DateTime.Today;
        }
        return true;
    }
}