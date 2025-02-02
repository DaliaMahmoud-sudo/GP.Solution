using GP.Core.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GP.Core.Entities
{
    public class XRayReport
    {
        public int Id { get; set; }
        public int ReportId { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public string PdfReport { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}
