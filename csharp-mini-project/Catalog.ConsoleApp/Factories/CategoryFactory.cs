using Catalog.ConsoleApp.Domain.Classes.Category;
namespace Catalog.ConsoleApp.Factories;

public static class CategoryFactory
{
    public static Category GetCategory(Dictionary<string, object> rowData)
    {
        string Id = (string)rowData["Id"];

        string Name = (string)rowData["Name"];

        string ParentCategoryId = (string)rowData["ParentCategoryId"];

        Category category = new Category(Id, Name, ParentCategoryId);

        return category;
    }
}