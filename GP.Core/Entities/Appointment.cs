using GP.Core.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GP.Core.Entities
{
    public class Appointment
    {
        public int AppointmentId { get; set; }
        
       public string UserId { get; set; }
        public User User { get; set; }
        public string DoctorId { get; set; }
        public Doctor Doctor { get; set; }
        public DateTimeOffset AppointmentDate { get; set; }
        public Status AppointmentStatus { get; set; }
        public string Note { get; set; }
        public DateTimeOffset CreatedAt { get; set; }


    }
}
