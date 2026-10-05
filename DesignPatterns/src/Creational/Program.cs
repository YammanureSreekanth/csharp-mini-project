using Creational.Singleton;

namespace Creational;

public class Program
{
    public static void Main()
    {
        // Task<NaiveSettings>[] tasks = Enumerable
        //     .Range(start: 0, 8)
        //     .Select(_ => Task.Run(() => NaiveSettings.Instance))
        //     .ToArray();
        // Task.WaitAll(tasks);
        // Console.WriteLine($"Instances created: {NaiveSettings.Created}");

        // Task<LockedSettings>[]? tasks = Enumerable
        //     .Range(0, 10)
        //     .Select(_ => Task.Run(() => LockedSettings.Instance))
        //     .ToArray();
        // Task.WaitAll(tasks);
        // Console.WriteLine($"Instances are created {LockedSettings.Created}");

        // Console.WriteLine("Before Creating");
        // Console.WriteLine(EagerSettings.Instance.Name);
        // Console.WriteLine(ReferenceEquals(EagerSettings.Instance, EagerSettings.Instance));

        // Console.WriteLine(value: $"Checking {nameof(LazySettings)} singleton");
        // LazySettings lazySettings1 = LazySettings.Instance;
        // LazySettings lazySettings2 = LazySettings.Instance;
        // Console.WriteLine(ReferenceEquals(lazySettings1, lazySettings2));
        // Console.WriteLine($"Name1: {lazySettings1.Name} Role1: {lazySettings1.Role}");
        // Console.WriteLine($"Name2: {lazySettings2.Name} Role2: {lazySettings2.Role}");

        // Console.WriteLine(value: $"Checking {nameof(Monostate)} singleton");
        // Monostate monostate1 = new Monostate();
        // Console.WriteLine($"Before Price1: {monostate1.Currency}{monostate1.ShopPrice}");
        // Monostate monostate2 = new Monostate();
        // Console.WriteLine($"Before Price2: {monostate2.Currency}{monostate2.ShopPrice}");
        // monostate1.Currency = "USD";
        // Console.WriteLine($"After Price1: {monostate1.Currency}{monostate1.ShopPrice}");
        // Console.WriteLine($"After Price2: {monostate2.Currency}{monostate2.ShopPrice}");
        // Console.WriteLine(ReferenceEquals(monostate1, monostate1));

        // Console.WriteLine(value: $"Checking {nameof(RequestTracer)} singleton");
        // RequestTracer? mine = RequestTracer.Current;
        // Console.WriteLine(ReferenceEquals(mine, RequestTracer.Current)); // same thread, same object
        // RequestTracer? other = null;
        // var thread = new Thread(() => other = RequestTracer.Current);
        // thread.Start();
        // thread.Join();
        // Console.WriteLine(ReferenceEquals(mine, other)); // other thread, own object

        Console.WriteLine(value: $"Checking {nameof(AmbientContext)} singleton");
        Console.WriteLine($"Current TaxRate: {AmbientContext.CurrentRate}");
        using (new AmbientContext(1.5m))
        {
            Console.WriteLine($"Inside First USING Current TaxRate: {AmbientContext.CurrentRate}");
            using (new AmbientContext(2.5m))
            {
                Console.WriteLine($"Nested USING Current TaxRate: {AmbientContext.CurrentRate}");
            }
            Console.WriteLine($"After Nested USING Current TaxRate: {AmbientContext.CurrentRate}");
        }
        Console.WriteLine($"Outside USING Current TaxRate: {AmbientContext.CurrentRate}");
    }
}
