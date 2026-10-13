namespace Creational.Factory;

public class Coupon
{
    public string Code { get; }
    public decimal PercentOff { get; }

    private Coupon(string code, decimal percentOff)
    {
        Code = code;
        PercentOff = percentOff;
    }

    public static class CouponFactory
    {
        public static Coupon Welcome() => new("WELCOME10", 10m);

        public static Coupon Custom(string code, decimal percentOff)
        {
            return new Coupon(code, percentOff);
        }
    }

    public override string ToString() => $"{Code} (-{PercentOff}%)";
}
