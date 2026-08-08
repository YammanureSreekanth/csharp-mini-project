namespace Domain.Structs
{
    public struct StockStatus
    {
        public int ATS {get; set;}
        public int PreOrder {get; set;}
        public DateOnly InstockDate {get; set;}
        public bool IsOrderable {get;}

        public bool IsPerectual {get; set;}
    }
}