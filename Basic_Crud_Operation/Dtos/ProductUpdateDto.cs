using System.ComponentModel.DataAnnotations;

namespace Basic_Crud_Operation.Dtos
{
    public class ProductUpdateDto
    {
        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Range(0.01, 1000000)]
        public decimal Price { get; set; }

        [Range(0, int.MaxValue)]
        public int Quantity { get; set; }
    }
}
