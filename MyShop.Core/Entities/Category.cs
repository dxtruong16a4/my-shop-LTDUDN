namespace MyShop.Core.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int? ParentId { get; set; } // For parent-child relationship (hierarchy)
        public int? OrderIndex { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public Category? ParentCategory { get; set; }
        public ICollection<Category> SubCategories { get; set; } = new List<Category>();
        public ICollection<Group> Groups { get; set; } = new List<Group>();
    }
}
