namespace Creational.Factory;

public interface INotifierRegistry
{
    string Send(string to, string message);
}

public class EmailNotifierRegistry : INotifierRegistry
{
    public string Send(string to, string message) => $"EMAIL to {to} : {message}";
}

public class SMSNotifierRegistry : INotifierRegistry
{
    public string Send(string to, string message) => $"SMS to {to} : {message}";
}

public class PushNotifierRegistry : INotifierRegistry
{
    public string Send(string to, string message) => $"PUSH to {to} : {message}";
}

public class NotifierRegistry
{
    private readonly Dictionary<string, Func<INotifierRegistry>> _makers = new();

    public NotifierRegistry Register(string channel, Func<INotifierRegistry> maker)
    {
        _makers[channel] = maker;
        return this;
    }

    public INotifierRegistry Create(string channel)
    {
        bool validMaker = _makers.TryGetValue(channel, out Func<INotifierRegistry>? maker);

        if (validMaker)
        {
            return maker();
        }

        throw new ArgumentException($"Unknown channel '{channel}'");
    }
}
