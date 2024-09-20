using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    public class ResetPasswordDto
    {


        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
        public string Token { get; set; }
    }

}
