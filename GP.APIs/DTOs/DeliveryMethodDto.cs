
using System.ComponentModel.DataAnnotations;

namespace GP.APIs.DTOs
{
    public class DeliveryMethodDto
    {
        public int Id { get; set; }
        [Required]
        [MinLength(3)]
        public string ShortName { get; set; }
        [Required]
        [MinLength(10)]
        public string Description { get; set; }
        [Required]
        [MinLength(4)]
        public string DeliveryTime { get; set; }
        [Required(ErrorMessage = "The Cost Field Is Required")]
        [Range(1, 10000)]
        public decimal Cost { get; set; }
    }
}
