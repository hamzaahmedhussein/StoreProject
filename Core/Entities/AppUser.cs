using Microsoft.AspNetCore.Identity;

namespace Core.Entities
{
    public class AppUser : IdentityUser
    {
        public string Name { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public string? OTP { get; set; }
        public DateTime? OTPExpiry { get; set; }

        public Address Address { get; set; }

    }
}

