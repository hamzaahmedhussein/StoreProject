
using Application.Services;
using Core.Entities.Identity;
using Microsoft.AspNetCore.Identity;

public class OtpService : IOtpService
{
    private readonly Dictionary<string, (string Otp, DateTime Expiration)> _otpStorage = new();
    private readonly IEmailService _emailService;
    private readonly UserManager<AppUser> _userManager;

    public OtpService(IEmailService emailService, UserManager<AppUser> userManager)
    {
        _emailService = emailService;
        _userManager = userManager;
    }

    public async Task<string> GenerateOtpAsync(string email)
    {
        var otp = new Random().Next(100000, 999999).ToString(); // Generate a 6-digit OTP
        var expiration = DateTime.UtcNow.AddMinutes(5); // OTP is valid for 5 minutes

        _otpStorage[email] = (otp, expiration);

        var subject = "Your OTP Code";
        var body = $"Your OTP code is {otp}. It is valid for 5 minutes.";
        await _emailService.SendEmailAsync(email, subject, body);

        return otp;
    }

    public Task<bool> VerifyOtpAsync(string email, string otp)
    {
        if (_otpStorage.TryGetValue(email, out var storedOtp) && storedOtp.Otp == otp)
        {
            if (DateTime.UtcNow <= storedOtp.Expiration)
            {
                _otpStorage.Remove(email);
                return Task.FromResult(true);
            }
            else
            {
                _otpStorage.Remove(email);
            }
        }

        return Task.FromResult(false);
    }

    public async Task<bool> ConfirmEmailWithOtpAsync(string email, string otp)
    {
        var isValid = await VerifyOtpAsync(email, otp);
        if (isValid)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var result = await _userManager.ConfirmEmailAsync(user, token);
                return result.Succeeded;
            }
        }

        return false;
    }

    public async Task<bool> ResetPasswordWithOtpAsync(string email, string otp, string newPassword)
    {
        var isValid = await VerifyOtpAsync(email, otp);
        if (isValid)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, resetToken, newPassword);
                return result.Succeeded;
            }
        }

        return false;
    }
}