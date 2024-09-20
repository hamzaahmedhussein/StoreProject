using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    public class ForgotPasswordDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }

}
