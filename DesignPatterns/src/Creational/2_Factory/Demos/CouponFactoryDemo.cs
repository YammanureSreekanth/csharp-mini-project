namespace Creational.Factory.Demos;

public sealed class CouponFactoryDemo : IDemo
{
    public string Category => "Factory";
    public string Name => "Inner factory class (coupons)";

    public Task RunAsync()
    {
        Console.WriteLine(Coupon.CouponFactory.Welcome());
        Console.WriteLine(Coupon.CouponFactory.Custom("SREEKANTH", 50));
        return Task.CompletedTask;
    }
}
