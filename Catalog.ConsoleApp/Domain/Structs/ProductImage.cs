namespace Catalog.ConsoleApp.Domain.Structs
{
    public struct ProductImage
    {
        public string Title {get; set;}
        public string Alt {get; set;}

        public string Path {get; set;}

        public ProductImage(string title, string alt, string path)
        {
            Title = title;
            Alt = alt;
            Path = path;
        }
    }
}