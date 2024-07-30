namespace Application.Services
{
    using Application.DTOs;
    using Application.Settings;
    using AutoMapper;
    using Core.Entities;
    using Core.Entities.Identity;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Logging;
    using System.Security.Claims;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading.Tasks;

    namespace Infrastructure.Services
    {
        public class AccountService : IAccountService
        {
            private readonly UserManager<AppUser> _userManager;
            private readonly ITokenService _tokenService;
            private readonly RoleManager<IdentityRole> _roleManager;
            private readonly ILogger<AccountService> _logger;
            private readonly IMapper _mapper;
            private readonly IHttpContextAccessor _contextAccessor;
            private readonly IMailingService _mailingService;

            public AccountService(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, ILogger<AccountService> logger, IMapper mapper, ITokenService tokenService, IHttpContextAccessor contextAccessor, IMailingService mailingService)
            {
                _userManager = userManager;
                _roleManager = roleManager;
                _logger = logger;
                _mapper = mapper;
                _tokenService = tokenService;
                _contextAccessor = contextAccessor;
                _mailingService = mailingService;
            }

            public async Task<IdentityResult> RegisterCustomerAsync(CustomerRegistrationDto customerDto)
            {
                var user = new Customer
                {
                    UserName = customerDto.Email,
                    Email = customerDto.Email,
                    DisplayName = customerDto.DisplayName,
                    Address = new Address
                    {
                        Street = customerDto.Street,
                        City = customerDto.City,
                        State = customerDto.State,
                    },
                    DateOfBirth = customerDto.DateOfBirth,
                };

                var result = await _userManager.CreateAsync(user, customerDto.Password);
                if (!result.Succeeded)
                {
                    _logger.LogError("User creation failed: {errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
                    return result;
                }

                await SendOTPAsync(customerDto.Email);


                await EnsureRoleExistsAsync("Customer");
                return await _userManager.AddToRoleAsync(user, "Customer");


            }

            public async Task<IdentityResult> RegisterSellerAsync(SellerRegistrationDto sellerDto)
            {
                var user = new Seller
                {
                    UserName = sellerDto.Email,
                    Email = sellerDto.Email,
                    DisplayName = sellerDto.DisplayName,
                    Address = new Address
                    {
                        Street = sellerDto.Street,
                        City = sellerDto.City,
                        State = sellerDto.State,
                    },
                };




                var result = await _userManager.CreateAsync(user, sellerDto.Password);
                if (!result.Succeeded)
                {
                    _logger.LogError("User creation failed: {errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
                    return result;
                }
                await SendOTPAsync(sellerDto.Email);
                _logger.LogInformation("OTP generated and sent for email: {email}", sellerDto.Email);
                await EnsureRoleExistsAsync("Seller");
                return await _userManager.AddToRoleAsync(user, "Seller");
            }


            public async Task<IdentityResult> ChangePassword(ChangePasswordDto changePasswordDto)
            {
                var user = await GetCurrentUserAsync();
                if (user == null)
                {
                    return IdentityResult.Failed(new IdentityError { Description = "User not found." });
                }

                var result = await _userManager.ChangePasswordAsync(user, changePasswordDto.OldPassword, changePasswordDto.NewPassword);
                return result;
            }

            public async Task<LoginResult> Login(LoginDto userDto)
            {
                var user = await _userManager.FindByEmailAsync(userDto.Email);
                if (user == null)
                {
                    return CreateLoginResult(false, null, LoginErrorType.UserNotFound);
                }

                if (!await _userManager.CheckPasswordAsync(user, userDto.Password))
                {
                    return CreateLoginResult(false, null, LoginErrorType.InvalidPassword);
                }

                if (!await _userManager.IsEmailConfirmedAsync(user))
                {
                    return CreateLoginResult(false, null, LoginErrorType.EmailNotConfirmed);
                }
                var token = _tokenService.CreateToken(user);

                return new LoginResult
                {
                    Success = true,
                    Token = token,
                };
            }

            private LoginResult CreateLoginResult(bool success, string token, LoginErrorType errorType)
            {
                return new LoginResult
                {
                    Success = success,
                    Token = token,
                    ErrorType = errorType
                };
            }
            private async Task EnsureRoleExistsAsync(string roleName)
            {
                if (!await _roleManager.RoleExistsAsync(roleName))
                {
                    var roleResult = await _roleManager.CreateAsync(new IdentityRole(roleName));
                    if (!roleResult.Succeeded)
                    {
                        _logger.LogError("Role creation failed: {errors}", string.Join(", ", roleResult.Errors.Select(e => e.Description)));
                    }
                }
            }
            private async Task<AppUser> GetCurrentUserAsync()
            {
                ClaimsPrincipal currentUser = _contextAccessor.HttpContext.User;
                return await _userManager.GetUserAsync(currentUser);
            }

            public async Task<bool> ForgotPasswordAsync(string email)
            {
                if (string.IsNullOrEmpty(email))
                {
                    throw new ArgumentException("Email cannot be null or empty.", nameof(email));
                }

                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    return false;
                }
                var result = await SendOTPAsync(email);

                return result.Succeeded;
            }

            public async Task<bool> ResetPasswordAsync(ResetPasswordDto model)
            {
                if (model == null)
                    throw new ArgumentNullException(nameof(model));

                if (string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Otp) || string.IsNullOrEmpty(model.NewPassword) || string.IsNullOrEmpty(model.ConfirmPassword))
                    throw new ArgumentException("Email, OTP, NewPassword, and ConfirmPassword cannot be null or empty.");

                if (model.NewPassword != model.ConfirmPassword)
                    return false;

                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    return false;
                }

                if (user.OTP != model.Otp || user.OTPExpiry < DateTime.UtcNow)
                {
                    return false;
                }

                var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, resetToken, model.NewPassword);
                if (result.Succeeded)
                {
                    user.OTP = string.Empty;
                    user.OTPExpiry = DateTime.MinValue;
                    await _userManager.UpdateAsync(user);
                    return true;
                }

                return false;
            }



            #region OTP Management
            public async Task<IdentityResult> SendOTPAsync(string email)
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                    return IdentityResult.Failed(new IdentityError { Description = "User not found" });

                var otp = GenerateOTP();
                user.OTP = otp;
                user.OTPExpiry = DateTime.UtcNow.AddMinutes(15);
                await _userManager.UpdateAsync(user);

                var subject = "your otp code";
                var body = $"your otp code is {otp}. it is valid for 5 minutes.";
                var message = new MailMessage(new[] { email }, subject, body);

                await _mailingService.SendMailAsync(message);

                return IdentityResult.Success;
            }

            public async Task<IdentityResult> ConfirmEmailWithOTP(VerifyOtpDto verifyOTPRequest)
            {
                var user = await _userManager.FindByEmailAsync(verifyOTPRequest.Email);
                if (user == null)
                    return IdentityResult.Failed(new IdentityError { Description = "User not found" });

                if (user.OTP != verifyOTPRequest.OTP || user.OTPExpiry < DateTime.UtcNow)
                    return IdentityResult.Failed(new IdentityError { Description = "Invalid or expired OTP" });

                user.OTP = string.Empty;
                user.OTPExpiry = DateTime.MinValue;
                user.EmailConfirmed = true;
                await _userManager.UpdateAsync(user);

                return IdentityResult.Success;
            }
            private string GenerateOTP()
            {
                using var rng = new RNGCryptoServiceProvider();
                var byteArray = new byte[6];
                rng.GetBytes(byteArray);

                var sb = new StringBuilder();
                foreach (var byteValue in byteArray)
                {
                    sb.Append(byteValue % 10);
                }
                return sb.ToString();
            }
            #endregion

        }
    }

}
