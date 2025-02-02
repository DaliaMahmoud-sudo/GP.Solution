using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entites.Identity;

namespace GP.Core.Entities.Identity
{
    public class AppUser : IdentityUser
    {
       
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Address Address { get; set; }
        public ICollection<Appointment> Appointments { get; set; } = new HashSet<Appointment>();
    }
}
