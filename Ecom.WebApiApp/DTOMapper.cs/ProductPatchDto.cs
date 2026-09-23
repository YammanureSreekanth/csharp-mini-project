namespace Ecom.WebApiApp.DTOMapper;

public class ProductPatchDto
{
    public string? Name {get; set;}
    public string? Description {get; set;}
    public bool? IsOnline {get; set;}
    public bool? IsSearchable {get; set;}
    public string? FabricCode {get; set;}
    public decimal? Price {get; set;}
}