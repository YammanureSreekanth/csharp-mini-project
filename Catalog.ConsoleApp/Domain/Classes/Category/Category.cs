namespace Catalog.ConsoleApp.Domain.Classes.Category
{
    public class Category
    {
        public string Id {get; init;}
        public string Name {get; init;}
        public Category? ParentCategory {get; protected set;}
        public List<Category>? SubCategories {get; protected set;}

        public Category(string id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}