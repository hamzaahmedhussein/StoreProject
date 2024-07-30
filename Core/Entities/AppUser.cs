using Microsoft.AspNetCore.Identity;

namespace Core.Entities.Identity
{
    public class AppUser : IdentityUser
    {
        public string DisplayName { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public string? OTP { get; set; }
        public DateTime? OTPExpiry { get; set; }

        public Address Address { get; set; }
    }
}
