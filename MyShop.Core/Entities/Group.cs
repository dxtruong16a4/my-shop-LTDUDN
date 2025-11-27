namespace MyShop.Core.Entities
{
    public class Group
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int CategoryId { get; set; }
        public string? Description { get; set; }

        // Navigation properties
        public Category? Category { get; set; }
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
