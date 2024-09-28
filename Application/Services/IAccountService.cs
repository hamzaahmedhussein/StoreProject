using Application.DTOs;
using Connect.Application.DTOs;
using Core.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
namespace Application.Services
{
    public interface IAccountService
    {
        Task<IdentityResult> RegisterSellerAsync(SellerRegistrationDto sellerDto);
        Task<IdentityResult> RegisterCustomerAsync(CustomerRegistrationDto customerDto);
        Task<LoginResult> Login(LoginUserDto userDto);
        Task<IdentityResult> ChangePassword(ChangePasswordDto changePasswordDto);
        Task<AppUser> GetCurrentUserAsync();
        Task<UserProfileInfo> GetCurrentCustomerAsync();
        Task<UserProfileInfo> GetCurrentSellerAsync();
        Task<IdentityResult> SendOTPAsync(string email);
        Task<IdentityResult> ConfirmEmailWithOTP(VerifyOtpDto verifyOTPRequest);
        Task<Result> ForgotPasswordAsync(string email);
        Task<string> VerifyOtpAndGenerateTokenAsync(string email, string otp);
        Task<Result> ResetPasswordAsync(ResetPasswordDto model);
        Task<string> AddProfilePictureAsync(IFormFile file);
        Task<string> UpdateProfilePictureAsync(IFormFile file);
        Task<bool> DeleteProfilePictureAsync();


    }
}
