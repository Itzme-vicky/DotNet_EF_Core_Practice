namespace Basic_Crud_Operation.Models
{
    public class Product
    {
        public int Id { get; set; }                  // Primary key (EF convention: "Id")
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
