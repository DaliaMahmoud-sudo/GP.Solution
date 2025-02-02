using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GP.Core.Entities.Identity
{
    public class AvailableTimes
    {
        public int Id { get; set; }
        public Doctor Doctor { get; set; }
        public string DoctorId { get; set; }
        public string Day { get; set; }
        public DateTimeOffset StartAt { get; set; }
        public DateTimeOffset EndAt { get; set; }
    }
}
