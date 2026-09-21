namespace Catalog.ConsoleApp.Domain.Classes.Category
{
    public class Category
    {
        public string Id {get; init;}
        public string Name {get; init;}
        public string? ParentCategoryId {get; init;}

        private readonly List<Category> _subCategories = new List<Category>();
        public IReadOnlyList<Category> SubCategories => _subCategories;

        public Category(string id, string name, string? parentCategoryId)
        {
            Id = id;
            Name = name;
            ParentCategoryId = parentCategoryId;
        }
    
        public void AddSubCategory(Category child)
        {
            _subCategories.Add(child);
        }

        public IEnumerable<Category> Childs()
        {
            foreach (Category sub in _subCategories)
            {
                yield return sub;
                foreach (Category c in sub.Childs())
                    yield return c;
            }
        }
    }
}