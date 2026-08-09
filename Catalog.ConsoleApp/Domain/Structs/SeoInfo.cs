namespace Catalog.ConsoleApp.Domain.Structs
{
    public struct SeoInfo
    {
        public string PageTitle {get; set;}
        public string? PageKeywords {get; set;}

        public SeoInfo(string pageTitle)
        {
            PageTitle = pageTitle;
        }
    }
}