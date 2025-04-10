using GP.Core.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GP.Core.Entities
{
    public class Payment
    {
         public int paymentId { get; set; }

        public string UserId { get; set; }
        public User User { get; set; }

        public string Name { get; set; }
        public string Email { get; set; }

        public decimal TotalPrice { get; set; }

        public DateTimeOffset PaymentDate { get; set; } = DateTimeOffset.UtcNow;



    }
}
