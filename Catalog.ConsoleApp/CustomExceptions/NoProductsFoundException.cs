namespace Catalog.ConsoleApp.CustomExceptions
{
    public class NoProductsFoundException: Exception
    {
        public NoProductsFoundException(string message): base(message) {}
    }
}
