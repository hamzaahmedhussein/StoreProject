using Application.DTOs;
using Connect.Application.DTOs;
using Core.Entities;
using Microsoft.AspNetCore.Identity;
namespace Application.Services
{
    public interface IAccountService
    {
        Task<IdentityResult> RegisterSellerAsync(SellerRegistrationDto sellerDto);
        Task<IdentityResult> RegisterCustomerAsync(CustomerRegistrationDto customerDto);
        Task<LoginResult> Login(LoginUserDto userDto);
        Task<IdentityResult> ChangePassword(ChangePasswordDto changePasswordDto);
        Task<IdentityResult> AddUserToRoleAsync(string email, string role);
        Task<AppUser> GetCurrentUserAsync();
        Task<IdentityResult> SendOTPAsync(string email);
        Task<IdentityResult> ConfirmEmailWithOTP(VerifyOtpDto verifyOTPRequest);
        Task<Result> ForgotPasswordAsync(string email);
        Task<string> VerifyOtpAndGenerateTokenAsync(string email, string otp);
        Task<Result> ResetPasswordAsync(ResetPasswordDto model);


    }
}
