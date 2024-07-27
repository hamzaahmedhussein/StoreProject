namespace Application.Services
{
    using Application.DTOs;
    using AutoMapper;
    using Core.Entities;
    using Core.Entities.Identity;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Logging;
    using System.Security.Claims;
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
            private readonly IOtpService _otpService;
            private readonly IHttpContextAccessor _contextAccessor;

            public AccountService(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, ILogger<AccountService> logger, IMapper mapper, ITokenService tokenService, IOtpService otpService, IHttpContextAccessor contextAccessor)
            {
                _userManager = userManager;
                _roleManager = roleManager;
                _logger = logger;
                _mapper = mapper;
                _tokenService = tokenService;
                _otpService = otpService;
                _contextAccessor = contextAccessor;
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
                var otp = await _otpService.GenerateOtpAsync(customerDto.Email);
                _logger.LogInformation("OTP generated and sent for email: {email}", customerDto.Email);
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
                var otp = await _otpService.GenerateOtpAsync(sellerDto.Email);
                _logger.LogInformation("OTP generated and sent for email: {email}", sellerDto.Email);
                await EnsureRoleExistsAsync("Seller");
                return await _userManager.AddToRoleAsync(user, "Seller");
            }

            public async Task<bool> ConfirmEmailAsync(string email, string otp)
            {
                return await _otpService.ConfirmEmailWithOtpAsync(email, otp);
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
                    return false; // User not found
                }

                await _otpService.GenerateOtpAsync(email); // Ensure this method handles its own exceptions

                return true; // OTP generation initiated
            }

            public async Task<bool> ResetPasswordAsync(ResetPasswordDto model)
            {
                if (model == null)
                {
                    throw new ArgumentNullException(nameof(model));
                }

                if (string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Otp) || string.IsNullOrEmpty(model.NewPassword))
                {
                    throw new ArgumentException("Email, OTP, and new password must be provided.");
                }

                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    return false; // User not found
                }

                var isOtpValid = await _otpService.VerifyOtpAsync(model.Email, model.Otp);
                if (!isOtpValid)
                {
                    return false; // Invalid OTP
                }

                var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                var resetResult = await _userManager.ResetPasswordAsync(user, resetToken, model.NewPassword);

                return resetResult.Succeeded;
            }


        }
    }

}
