namespace Creational.Builder.Demos;

public sealed class ParcelBuilderDemo : IDemo
{
    public string Category => "Builder";
    public string Name => "Stepwise builder (parcel)";

    public Task RunAsync()
    {
        Parcel parcel = ParcelBuilder
            .Create()
            .UseCarrier("UPS")
            .WithWeight(2.5)
            .DeliverTo("Almere")
            .Build();
        Console.WriteLine(parcel);
        return Task.CompletedTask;
    }
}
