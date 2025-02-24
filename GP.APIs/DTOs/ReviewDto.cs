using GP.Core.Entities.Identity;

namespace GP.APIs.DTOs
{
    public class ReviewDto
    {

        public string UserId { get; set; }
        public string DoctorId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
    }
}
