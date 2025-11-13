namespace MyShop.Core.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int GroupId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int Stock { get; set; }
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public Group Group { get; set; }
    }
}
