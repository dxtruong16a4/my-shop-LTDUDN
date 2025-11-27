namespace MyShop.Core.DTOs
{
    public class CategoryDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int? ParentId { get; set; }
        public int? OrderIndex { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateCategoryDto
    {
        public string? Name { get; set; }
        public int? ParentId { get; set; }
        public int? OrderIndex { get; set; }
    }

    public class UpdateCategoryDto
    {
        public string? Name { get; set; }
        public int? ParentId { get; set; }
        public int? OrderIndex { get; set; }
    }
}
