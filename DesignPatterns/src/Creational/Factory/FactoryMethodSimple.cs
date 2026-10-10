namespace Creational.Factory;

public interface INotifier
{
    void Send(string to, string message);
}

public class EmailNotifier : INotifier
{
    public void Send(string to, string message) => Console.WriteLine($"EMAIL to {to} : {message}");
}

public class SMSNotifier : INotifier
{
    public void Send(string to, string message) => Console.WriteLine($"SMS to {to} : {message}");
}

public class PushNotifier : INotifier
{
    public void Send(string to, string message) => Console.WriteLine($"PUSH to {to} : {message}");
}

public class NotificationFactory
{
    public static INotifier Create(string channel) =>
        channel.ToLowerInvariant() switch
        {
            "email" => new EmailNotifier(),
            "sms" => new SMSNotifier(),
            "push" => new PushNotifier(),
            _ => throw new NotImplementedException(),
        };
}
