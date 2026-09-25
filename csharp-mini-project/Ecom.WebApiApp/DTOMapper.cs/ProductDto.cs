namespace Ecom.WebApiApp.DTOMapper;

public class ProductDto
{
    public string Id {get; set;}
    public string Name {get; set;}
    public string Description {get; set;}
    public bool IsOnline {get; set;}
    public bool IsSearchable {get; set;}
    public string FabricCode {get; set;}
    public decimal Price {get; set;}
}