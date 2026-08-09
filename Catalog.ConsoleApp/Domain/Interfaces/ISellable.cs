using Catalog.ConsoleApp.Domain.Structs;

namespace Catalog.ConsoleApp.Domain.Interfaces
{
    public interface ISellable
    {
            Money Price { get; }
            StockStatus Availability { get; }
    }
}