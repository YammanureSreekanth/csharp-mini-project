using Domain.Enums;
using Domain.Interfaces;
using Domain.Structs;

namespace Domain.Classes.Product
{
    public abstract class BaseProduct: ISearchable, IPriceable
    {
        public string Id {get; init;}
        public string Name {get; set;}
        public bool IsOnline {get; set;}
        public bool IsSearchable {get; set;}
        public string ShortDescription {get; set;}
        public SeoInfo SEO {get; set;}
        public ProductType Type {get; set;}
        public IReadOnlyList<ProductImage> Images {get; set;}
        public abstract string GetPriceDisplay();
        public abstract bool MatchesKeyword(string keyword);
        public BaseProduct(string id, string name, ProductType type)
        {
           Id = id;
           Name = name;
           Type = type;
        }
    }
}