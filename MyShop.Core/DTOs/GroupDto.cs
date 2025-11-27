namespace MyShop.Core.DTOs
{
    public class GroupDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int CategoryId { get; set; }
        public string? Description { get; set; }
    }

    public class CreateGroupDto
    {
        public string? Name { get; set; }
        public int CategoryId { get; set; }
        public string? Description { get; set; }
    }

    public class UpdateGroupDto
    {
        public string? Name { get; set; }
        public int CategoryId { get; set; }
        public string? Description { get; set; }
    }
}
