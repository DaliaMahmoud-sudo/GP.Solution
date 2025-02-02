using GP.Core.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GP.Core.Entities
{
    public class Review
    {
       public int ReviewId { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }

    }
}
