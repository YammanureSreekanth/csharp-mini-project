namespace Ecom.WebApiApp.Models;

public class Product
{
    public string Id {get; set;}
    public string Name {get; set;}
    public string Description {get; set;}
    public bool IsOnline {get; set;}
    public bool IsSearchable {get; set;}
    public string FabricCode {get; set;}
    public decimal Price {get; set;}
    public DateTime CreatedTime {get; set;}
    public DateTime LastModifiedTime {get; set;}
}