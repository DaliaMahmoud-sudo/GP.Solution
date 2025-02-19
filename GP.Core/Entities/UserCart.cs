using GP.Core.Entities.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GP.Core.Entities
{
    public class UserCart
    {


       // public UserCart() { }

        // Constructor with 'id' parameter (optional, if needed elsewhere)
        public UserCart()
        {

            Items = new List<CartItems>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }  // Cart ID (same as User ID)

        //  Foreign Key to User
        public string UserId { get; set; }
        public User User { get; set; }  // Navigation property
        public List<CartItems> Items { get; set; } 




    }
}
