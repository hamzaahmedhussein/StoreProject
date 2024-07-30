using Application.DTOs;
using Microsoft.AspNetCore.Identity;

namespace Application.Services
{
    public interface IAccountService
    {
        Task<IdentityResult> RegisterCustomerAsync(CustomerRegistrationDto customerDto);
        Task<IdentityResult> RegisterSellerAsync(SellerRegistrationDto sellerDto);
        Task<LoginResult> Login(LoginDto userDto);
        Task<IdentityResult> ChangePassword(ChangePasswordDto changePasswordDto);
        Task<bool> ResetPasswordAsync(ResetPasswordDto model);
        Task<bool> ForgotPasswordAsync(string email);
        Task<IdentityResult> SendOTPAsync(string email);
        Task<IdentityResult> ConfirmEmailWithOTP(VerifyOtpDto verifyOTPRequest);


    }
}
