using Domain.Classes.Product;

namespace Domain.Classes.Category
{
    public class ProductCategoryAssignment
    {
        public BaseProduct Product {get; set;}
        public Category Category {get; set;}
        public bool IsPrimary {get; set;}

    }
}