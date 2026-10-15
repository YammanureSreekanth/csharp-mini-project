using ProductCatalog.App.Clients.Interfaces;

namespace ProductCatalog.Infra.Clients;

public sealed class ProductImageClientFactory(ContosoImageClient contoso, FabrikamImageClient fabrikam) : IProductImageClientFactory
{
    string Provider = "contoso";

    public IProductImageClient Resolve()
    {
        if (Provider == "contoso")
        {
            return contoso;
        }
        return fabrikam;
    }
}
