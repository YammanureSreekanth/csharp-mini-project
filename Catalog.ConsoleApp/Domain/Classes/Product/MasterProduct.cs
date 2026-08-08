using Domain.Enums;

namespace Domain.Classes.Product
{
    public class MasterProduct : BaseProduct
    {
        public List<string> VariationAttributes;
        public MasterProduct(string id, string name, List<string> variationAttributes)
        : base(id, name,ProductType.MASTER_PRODUCT )
        {
            VariationAttributes = variationAttributes;
        }
        public List<VariationProduct> Variations;

        public override string GetPriceDisplay()
        {
            throw new NotImplementedException();
        }

        public override bool MatchesKeyword(string keyword)
        {
            return Name.Contains(keyword);
        }
    }
}