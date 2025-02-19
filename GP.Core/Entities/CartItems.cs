using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GP.Core.Entities
{
    public class CartItems
    {
        public int Id { get; set; }
        public string productName { get; set; }
        public string ImageUrl { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        // Foreign Key to UserCart
        [JsonIgnore]
        public int UserCartId { get; set; }
        [JsonIgnore]
        [ForeignKey("UserCartId")]
        public UserCart UserCart { get; set; }

    }
}
