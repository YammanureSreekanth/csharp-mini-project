using Domain.Structs;

namespace Domain.Interfaces
{
    public interface ISellable
    {
            Money Price { get; }
            StockStatus Availability { get; }
    }
}