namespace Creational;

public interface IDemo
{
    string Category { get; }   // "Singleton", "Factory", "AbstractFactory", "Builder"
    string Name { get; }
    Task RunAsync();
}