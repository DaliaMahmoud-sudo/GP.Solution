using System.ComponentModel.DataAnnotations;

namespace GP.APIs.DTOs
{
    public class AppointmentRequestDto
    {
        [Required]
        public string DoctorId { get; set; }

        [Required]
        public DateTimeOffset AppointmentDate { get; set; }

        public string Notes { get; set; }
    }
}
