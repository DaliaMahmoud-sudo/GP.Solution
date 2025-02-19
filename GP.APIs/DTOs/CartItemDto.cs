using System.ComponentModel.DataAnnotations;

namespace GP.APIs.DTOs
{
    public class  CartItemDto
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string productName { get; set; }
        [Required]
        public string ImageUrl { get; set; }
     
        [Required]
        public decimal Price { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage="must be one item at least")]
        public int Quantity { get; set; }
    }
}