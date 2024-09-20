using System.ComponentModel.DataAnnotations;

namespace Connect.Application.DTOs
{
    public class RegisterUserDto
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public String PhoneNumber { get; set; }

        public string Email { get; set; }
        [Required]

        public string Password { get; set; }
        [Required]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }
        public string Role { get; set; }

    }
}
