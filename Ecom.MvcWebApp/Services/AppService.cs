namespace Services;

public interface IMessageService
{
    string GetMessage();
}

public interface ILifetimeService
{
    Guid Id {get;}
}

public class MessageService: IMessageService
{
    public string GetMessage()
    {
        return "Hello from MessageService Service";
    }
}

public class TransientService: ILifetimeService
{
    public Guid Id { get;} = Guid.NewGuid(); 
}

public class ScopedService: ILifetimeService
{
    public Guid Id { get;} = Guid.NewGuid(); 
}

public class SingletonService: ILifetimeService
{
    public Guid Id { get;} = Guid.NewGuid(); 
}