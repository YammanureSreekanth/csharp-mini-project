namespace Creational.Builder;

public class Parcel
{
    public string Carrier = "";
    public double WeightKg;
    public string Address = "";

    public override string ToString() => $"{Carrier}, {WeightKg} kg, to {Address}";
}

public interface IBuildStep
{
    Parcel Build();
}

public interface IAddressStep
{
    public IBuildStep DeliverTo(string address);
}

public interface IWeightStep
{
    IAddressStep WithWeight(double weight);
}

public interface ICarrierStep
{
    IWeightStep UseCarrier(string carrier);
}

public static class ParcelBuilder
{
    private class Impl : ICarrierStep, IWeightStep, IAddressStep, IBuildStep
    {
        private readonly Parcel _parcel = new Parcel();

        public IWeightStep UseCarrier(string carrier)
        {
            _parcel.Carrier = carrier;
            return this;
        }

        public IBuildStep DeliverTo(string address)
        {
            _parcel.Address = address;
            return this;
        }

        public IAddressStep WithWeight(double weight)
        {
            _parcel.WeightKg = weight;
            return this;
        }

        public Parcel Build() => _parcel;
    }

    public static ICarrierStep Create() => new Impl();
}
