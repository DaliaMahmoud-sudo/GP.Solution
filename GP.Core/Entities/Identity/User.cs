using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entites.Identity;

namespace GP.Core.Entities.Identity
{
    public class User : AppUser
    {
     
        public DateTimeOffset CreatedAt { get; set; }
       
        public ICollection<XRayReport> XRayReports { get; set; } = new HashSet<XRayReport>();
        public ICollection<Notification> Notifications { get; set; } = new HashSet<Notification>();
        public ICollection<Review> Reviews { get; set; }= new HashSet<Review>();
        public UserCart Cart { get; set; }
        public ICollection<Appointment> Appointments { get; set; } = new HashSet<Appointment>();

    }
}
