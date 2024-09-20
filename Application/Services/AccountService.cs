﻿using Application.DTOs;
using Application.Settings;
using AutoMapper;
using Connect.Application.DTOs;
using Core.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Security.Cryptography;
using System.Text;

namespace Application.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IMailingService _mailingService;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ITokenService _tokenService;
        public AccountService(UserManager<AppUser> userManager,
            IMapper mapper, IHttpContextAccessor contextAccessor,
            IMailingService mailingService,
            RoleManager<IdentityRole> roleManager, ITokenService tokenService)
        {
            _userManager = userManager;
            _mapper = mapper;
            _contextAccessor = contextAccessor;
            _mailingService = mailingService;
            _roleManager = roleManager;
            _tokenService = tokenService;
        }

        #region Register

        public async Task<IdentityResult> RegisterCustomerAsync(CustomerRegistrationDto customerDto)
        {
            if (customerDto == null)
                throw new ArgumentNullException(nameof(customerDto));

            var customer = new Customer
            {
                UserName = customerDto.Email,
                Email = customerDto.Email,
                Name = customerDto.DisplayName,
                Address = new Address
                {
                    Street = customerDto.Street,
                    City = customerDto.City,
                    State = customerDto.State,
                },
                DateOfBirth = customerDto.DateOfBirth
            };

            var result = await _userManager.CreateAsync(customer, customerDto.Password);
            if (!result.Succeeded)
                return result;

            await EnsureRoleExistsAsync("Customer");
            await _userManager.AddToRoleAsync(customer, "Customer");

            IdentityResult sendConfirmationResult = await SendOTPAsync(customerDto.Email);
            if (!sendConfirmationResult.Succeeded)
                return sendConfirmationResult;

            return IdentityResult.Success;
        }

        public async Task<IdentityResult> RegisterSellerAsync(SellerRegistrationDto sellerDto)
        {
            if (sellerDto == null)
                throw new ArgumentNullException(nameof(sellerDto));

            var seller = new Seller
            {
                UserName = sellerDto.Email,
                Email = sellerDto.Email,
                Name = sellerDto.DisplayName,
                Address = new Address
                {
                    Street = sellerDto.Street,
                    City = sellerDto.City,
                    State = sellerDto.State,
                },
            };

            var result = await _userManager.CreateAsync(seller, sellerDto.Password);
            if (!result.Succeeded)
                return result;

            await EnsureRoleExistsAsync("Seller");
            await _userManager.AddToRoleAsync(seller, "Seller");

            IdentityResult sendConfirmationResult = await SendOTPAsync(sellerDto.Email);
            if (!sendConfirmationResult.Succeeded)
                return sendConfirmationResult;

            return IdentityResult.Success;
        }









        private async Task<IdentityResult> CreateUserAsync(RegisterUserDto userDto)
        {
            var existingUser = await _userManager.FindByEmailAsync(userDto.Email);
            if (existingUser != null)
                return IdentityResult.Failed(new IdentityError { Description = "User with this email already exists." });

            var user = _mapper.Map<AppUser>(userDto);
            return await _userManager.CreateAsync(user, userDto.Password);

        }

        private async Task EnsureRoleExistsAsync(string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
            {
                throw new ArgumentException("Role name cannot be null or empty.", nameof(roleName));
            }

            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                var roleResult = await _roleManager.CreateAsync(new IdentityRole(roleName));
                if (!roleResult.Succeeded)
                {
                    var errorMessage = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                    throw new InvalidOperationException($"Failed to create role '{roleName}'. Errors: {errorMessage}");
                }
            }
        }

        #endregion

        #region Login
        public async Task<LoginResult> Login(LoginUserDto userDto)
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

            var claims = await _tokenService.CreateClaimsForUserAsync(user);
            return await _tokenService.GenerateJwtTokenAsync(claims);



        }

        private LoginResult CreateLoginResult(bool success, string token, LoginErrorType errorType)
        {
            return new LoginResult
            {
                Success = success,
                Token = token,
                Expiration = default,
                ErrorType = errorType
            };
        }
        #endregion

        #region GetUserAndAddRole 
        public async Task<IdentityResult> AddUserToRoleAsync(string email, string role)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = $"User with email {email} not found." });
            }

            return await _userManager.AddToRoleAsync(user, role);
        }
        public async Task<AppUser> GetCurrentUserAsync()
        {
            var currentUser = _contextAccessor.HttpContext?.User;
            if (currentUser == null)
                return null;

            return await _userManager.GetUserAsync(currentUser);
        }
        #endregion

        #region ChangePassword
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
        #endregion

        public async Task<Result> ForgotPasswordAsync(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return Result.Failure("Email cannot be null or empty.");
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return Result.Failure("User not found.");
            }

            var result = await SendOTPAsync(email);
            if (result.Succeeded)
            {
                return Result.Success();
            }

            return Result.Failure("Failed to send OTP.");
        }

        public async Task<string> VerifyOtpAndGenerateTokenAsync(string email, string otp)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(otp))
                throw new ArgumentException("Email and OTP cannot be null or empty.");

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return null;

            if (user.OTP != otp || user.OTPExpiry < DateTime.UtcNow)
                return null;

            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            return resetToken;
        }
        public async Task<Result> ResetPasswordAsync(ResetPasswordDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return Result.Failure("User not found.");
            }

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);

            if (result.Succeeded)
            {
                await ClearOtpAsync(user);
                return Result.Success();
            }

            return Result.Failure("Failed to reset password.");
        }


        private async Task ClearOtpAsync(AppUser user)
        {
            user.OTP = string.Empty;
            user.OTPExpiry = DateTime.MinValue;
            await _userManager.UpdateAsync(user);
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
            var message = new Settings.MailMessage(new[] { email }, subject, body);

            _mailingService.SendMail(message);

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