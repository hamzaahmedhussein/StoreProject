namespace Application.Services
{
    public interface IOtpService
    {
        Task<string> GenerateOtpAsync(string email);
        Task<bool> VerifyOtpAsync(string email, string otp);
        Task<bool> ConfirmEmailWithOtpAsync(string email, string otp);
        Task<bool> ResetPasswordWithOtpAsync(string email, string otp, string newPassword);
    }

}
