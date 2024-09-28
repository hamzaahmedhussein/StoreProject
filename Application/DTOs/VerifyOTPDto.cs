using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    public class VerifyOtpDto
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string OTP { get; set; }
    }
}
