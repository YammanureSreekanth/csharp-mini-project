namespace Catalog.ConsoleApp.CustomAttributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class InfoAttribute: Attribute
    {
        public string Author {get;}
        public string Version {get;}

        public InfoAttribute(string author, string version)
        {
            Author = author;
            Version = version;
        }
    }
}