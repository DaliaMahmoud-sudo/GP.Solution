using GP.Core.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GP.Core.Entities
{
    public class UserCart
    {
        public UserCart(string id)
        {
            Id = id;
        }

        public string Id { get; set; }
        public List<CartItems> Items { get; set; }

    /*    public int UserId { get; set; }
        public User User { get; set; }*/
       
    }
}
