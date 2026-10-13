namespace Creational.Builder;

public class Customer
{
    public string Name = "";
    public string Email = "";
    public string Tier = "Standard";

    public override string ToString() => $"{Name} <{Email}> [{Tier}]";
}

public abstract class FunctionalBuilder<TSubject, TSelf>
    where TSubject : new()
    where TSelf : FunctionalBuilder<TSubject, TSelf>
{
    private readonly List<Func<TSubject, TSubject>> _actions = new();

    // Public so extension methods in other files can add steps.
    public TSelf Do(Action<TSubject> action)
    {
        _actions.Add(s =>
        {
            action(s);
            return s;
        }); // stored now, executed later
        return (TSelf)this;
    }

    public TSubject Build() => _actions.Aggregate(new TSubject(), (subject, step) => step(subject));
}

public sealed class CustomerBuilder : FunctionalBuilder<Customer, CustomerBuilder>
{
    public CustomerBuilder Called(string name) => Do(c => c.Name = name);
}

// Added later, from outside. CustomerBuilder itself is never edited.
public static class CustomerBuilderExtensions
{
    public static CustomerBuilder WithEmail(this CustomerBuilder b, string email) =>
        b.Do(c => c.Email = email);

    public static CustomerBuilder InTier(this CustomerBuilder b, string tier) =>
        b.Do(c => c.Tier = tier);
}
