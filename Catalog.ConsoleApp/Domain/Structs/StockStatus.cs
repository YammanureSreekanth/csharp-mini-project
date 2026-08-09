using Domain.Enums;
namespace Domain.Structs
{
    public struct StockStatus
    {
        public int ATS {get; set;}
        public int Preorder {get; set;}
        public DateOnly InstockDate {get; set;}
        public bool IsPerectual {get; set;}

        public AvailableStatus Status
        {
            get
            {
                if (ATS > 0) return AvailableStatus.IN_STOCK;
                if (Preorder > 0) return AvailableStatus.PRE_ORDER;
                return AvailableStatus.OUT_OF_STOCK;
            }
        }

        public bool IsOrderable
        {
            get
            {
                if (ATS > 0 || Preorder > 0) return true;
                return false;
            }
        }

        public StockStatus(int ats, int preoorder, bool isPerectual)
        {
            ATS = ats;
            Preorder = preoorder;
            IsPerectual = isPerectual;
        }
    }
}