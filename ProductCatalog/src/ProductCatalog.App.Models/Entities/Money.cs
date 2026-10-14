using ProductCatalog.App.Models.Exceptions;

namespace ProductCatalog.App.Models.Entities;

public enum Currency
{
    EUR,
}

public sealed record Money(decimal Amount, Currency Currency)
{
    public decimal Amount { get; } =
        Amount >= 0
            ? Amount
            : throw new ProductValidationException(
                "Money should be valid and more than or equal 0 "
            );
}
