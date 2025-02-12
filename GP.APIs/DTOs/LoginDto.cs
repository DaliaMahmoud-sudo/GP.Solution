using System.ComponentModel.DataAnnotations;

namespace GP.APIs.DTOs
{
    public class LoginDto
    {
        public int Id { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        public bool RememberMe { get; set; }

    }
}
