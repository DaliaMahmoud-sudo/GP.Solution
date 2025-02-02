using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entites.Identity;

namespace GP.Core.Entities.Identity
{
    public class Doctor : AppUser
    {
      
        public string Specializtion { get; set; }
       
        public string ImageUrl { get; set; }
        public string Bio { get; set; }
     
        public ICollection<AvailableTimes> AvailableTimes { get; set; } = new HashSet<AvailableTimes>();

        public ICollection<Appointment> Appointments { get; set; } = new HashSet<Appointment>();

        public ICollection<Review> Reviews { get; set; } = new HashSet<Review>();

    }
}
