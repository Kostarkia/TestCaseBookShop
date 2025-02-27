using TestCaseBookShop.Models.Data.Base;

namespace TestCaseBookShop.Models.Data
{
    public class Category : BaseEntity
    {
        public string Name { get; set; } = default!;
    }
}
