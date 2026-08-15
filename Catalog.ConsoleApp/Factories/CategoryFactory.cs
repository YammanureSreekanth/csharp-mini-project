using Catalog.ConsoleApp.Domain.Classes.Category;
using Microsoft.Data.SqlClient;

namespace Catalog.ConsoleApp.Factories;

public static class CategoryFactory
{
    public static Category GetCategory(Dictionary<string, object> rowData)
    {
        string Id = (string)rowData["Id"];
        string Name = (string)rowData["Name"];
        Category category = new Category(Id, Name);
        return category;
    }
}