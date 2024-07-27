using Application.DTOs;
using Application.Services;
using AutoMapper;
using Core.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly IAccountService _accountService;
        private readonly ILogger<AccountController> _logger;
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly IOtpService _otpService;

        public AccountController(IAccountService accountService, ILogger<AccountController> logger, IMapper mapper, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ITokenService tokenService, IOtpService otpService)
        {
            _accountService = accountService;
            _logger = logger;
            _mapper = mapper;
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _otpService = otpService;
        }

        [HttpPost]
        [Route("register/customer")]
        public async Task<IActionResult> RegisterCustomer([FromBody] CustomerRegistrationDto customerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _accountService.RegisterCustomerAsync(customerDto);
            if (!result.Succeeded)
            {
                _logger.LogError("Customer registration failed: {errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
                return StatusCode(500, "Customer registration failed.");
            }

            return Ok("Customer registered successfully.");
        }



        [HttpPost]
        [Route("register/seller")]
        public async Task<IActionResult> RegisterSeller([FromBody] SellerRegistrationDto sellerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _accountService.RegisterSellerAsync(sellerDto);
            if (!result.Succeeded)
            {
                _logger.LogError("Seller registration failed: {errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
                return StatusCode(500, "Seller registration failed.");
            }

            return Ok("Seller registered successfully.");
        }
        [HttpPost]
        [Route("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromBody] VerifyOtpDto request)
        {
            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Otp))
            {
                return BadRequest("Email and OTP are required.");
            }

            var isConfirmed = await _accountService.ConfirmEmailAsync(request.Email, request.Otp);
            if (isConfirmed)
            {
                return Ok(new { Message = "Email confirmed successfully." });
            }
            else
            {
                return BadRequest("Invalid OTP or email confirmation failed.");
            }
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var changePasswordResult = await _accountService.ChangePassword(changePasswordDto);
            if (!changePasswordResult.Succeeded)
            {
                foreach (var error in changePasswordResult.Errors)
                {
                    ModelState.AddModelError(error.Code, error.Description);
                }

                return BadRequest(ModelState);
            }

            return Ok("Password changed successfully");
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            if (email == null || string.IsNullOrEmpty(email))
            {
                return BadRequest("Email is required.");
            }

            var result = await _accountService.ForgotPasswordAsync(email);

            if (result)
            {
                return Ok("Password reset instructions have been sent.");
            }
            else
            {
                return NotFound("User not found.");
            }
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto model)
        {
            if (model == null || string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Otp) || string.IsNullOrEmpty(model.NewPassword))
            {
                return BadRequest("All fields are required.");
            }

            var result = await _accountService.ResetPasswordAsync(model);

            if (result)
            {
                return Ok("Password has been reset successfully.");
            }
            else
            {
                return BadRequest("Invalid OTP or password reset failed.");
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto userDto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid model state in login request.");
                return BadRequest(ModelState);
            }

            try
            {
                _logger.LogInformation("Processing login request for email: {Email}", userDto.Email);

                var result = await _accountService.Login(userDto);

                if (result.Success)
                {
                    _logger.LogInformation("Login successful for email: {Email}", userDto.Email);
                    return Ok(result);
                }

                (string errorMessage, int statusCode) = result.ErrorType switch
                {
                    LoginErrorType.UserNotFound => ("User not found.", 404),
                    LoginErrorType.InvalidPassword => ("Incorrect password.", 401),
                    LoginErrorType.EmailNotConfirmed => ("Email not confirmed.", 403),
                    _ => ("Invalid login attempt.", 400)
                };

                _logger.LogError("Login failed for email: {Email}. Error: {Error}", userDto.Email, errorMessage);

                return StatusCode(statusCode, new { message = errorMessage });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the login request for email: {Email}", userDto.Email);
                return StatusCode(500, new { message = "An unexpected error occurred. Please try again later." });
            }
        }


        //private async Task<ActionResult<AddressDto>> GetUserAddress()
        //{
        //    var user = await _userManager.FindUserByClaimsPrincipleWithAddress(User);
        //    return _mapper.Map<Address, AddressDto>(user.Address);
        //}


        //[Authorize]
        //[HttpPut("address")]
        //public async Task<ActionResult<AddressDto>> UpdateUserAddress(AddressDto address)
        //{
        //    var user = await _userManager.FindUserByClaimsPrincipleWithAddress(User);
        //    user.Address = _mapper.Map<AddressDto, Address>(address);
        //    var result = await _userManager.UpdateAsync(user);

        //    if (result.Succeeded) return Ok(address);

        //    return BadRequest("Problem updating the user");
        //}
    }
}
