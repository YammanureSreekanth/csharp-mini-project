using Catalog.ConsoleApp.Domain.Classes.Category;
using Catalog.ConsoleApp.Domain.Enums;
using Catalog.ConsoleApp.Domain.Interfaces;
using Catalog.ConsoleApp.Domain.Structs;

namespace Catalog.ConsoleApp.Domain.Classes.Product
{
    public abstract class BaseProduct: ISearchable, IPriceable
    {
        public string Id {get; init;}
        public string Name {get; set;}
        public bool IsOnline {get; set;}
        public bool IsSearchable {get; set;}
        public string? ShortDescription {get; set;}
        public SeoInfo SEO {get; set;}
        public ProductType Type {get; set;}
        private readonly List<ProductImage> _images = new List<ProductImage>();
        private readonly List<ProductCategoryAssignment> _categoryAssignment = new List<ProductCategoryAssignment>();

        public IReadOnlyList<ProductImage> Images => _images;
        public IReadOnlyList<ProductCategoryAssignment> CategoryAssignment => _categoryAssignment;

        public void AddImages(ProductImage image)
        {
            _images.Add(image);
        }

        public void AssignCateggory(ProductCategoryAssignment productCategory)
        {
            _categoryAssignment.Add(productCategory);
        }

        private static void MoveToFront<T>(IList<T> list, int index)
        {
            for (int i = index; i > 0; i--)
            {
                T temp = list[i];
                list[i] = list[i - 1];
                list[i - 1] = temp;
            }
        }

        public void SetPrimaryImage(int index)
        {
            if (index < 0 || index >= _images.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            MoveToFront(_images, index);
        }

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