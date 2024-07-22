namespace DotnetGraphQLCRUD.Model
{
    public class Product
    {
        public int Id { get; set; } = default!;
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? Category { get; set; }

    }
}
