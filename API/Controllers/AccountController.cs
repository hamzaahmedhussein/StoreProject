using Application.DTOs;
using Application.Services;
using Connect.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace YourNamespace.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly ILogger<AccountController> _logger;
        public AccountController(IAccountService accountService, ILogger<AccountController> logger)
        {
            _accountService = accountService;
            _logger = logger;
        }

        #region Register 
        [HttpPost("register-customer")]
        public async Task<IActionResult> RegisterCustomer([FromBody] CustomerRegistrationDto customerDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _accountService.RegisterCustomerAsync(customerDto);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return BadRequest(ModelState);
            }

            return Ok(new { message = "Customer registered successfully, please check your email for OTP confirmation." });
        }


        [HttpPost("register-seller")]
        public async Task<IActionResult> RegisterSeller([FromBody] SellerRegistrationDto sellerDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _accountService.RegisterSellerAsync(sellerDto);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return BadRequest(ModelState);
            }

            return Ok(new { message = "Seller registered successfully, please check your email for OTP confirmation." });
        }

        #endregion

        #region Login

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserDto userDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {

                var result = await _accountService.Login(userDto);

                if (result.Success)
                {
                    return Ok(result);
                }

                (string errorMessage, int statusCode) = result.ErrorType switch
                {
                    LoginErrorType.UserNotFound => ("User not found.", 404),
                    LoginErrorType.InvalidPassword => ("Incorrect password.", 401),
                    LoginErrorType.EmailNotConfirmed => ("Email not confirmed.", 403),
                    _ => ("Invalid login attempt.", 400)
                };


                return StatusCode(statusCode, new { message = errorMessage });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred. Please try again later." });
            }
        }


        [HttpPost("confirmemail")]

        public async Task<IActionResult> ConfirmEmail(VerifyOtpDto verifyDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var isConfirmed = await _accountService.ConfirmEmailWithOTP(verifyDto);
            if (isConfirmed.Succeeded)
            {
                return Ok(new { Message = "Email confirmed successfully." });
            }
            else
            {
                return BadRequest("Invalid OTP or email confirmation failed.");
            }

        }



        #endregion

        #region ChangePassword 
        [Authorize]
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


        #endregion

        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfileAsync()
        {
            var result = await _accountService.GetCurrentUserAsync();
            if (result != null)
            {
                return Ok(result);
            }

            return NotFound("User profile not found.");
        }

        #region ForgetPassword

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest("Email is required.");
            }

            var result = await _accountService.ForgotPasswordAsync(email);

            if (result.Succeeded)
            {
                return Ok("OTP has been sent to the provided email.");
            }

            return BadRequest(result.Message);
        }

        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp(string email, string otp)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(otp))
            {
                return BadRequest("Email and OTP are required.");
            }

            var token = await _accountService.VerifyOtpAndGenerateTokenAsync(email, otp);

            if (string.IsNullOrEmpty(token))
                return BadRequest();

            return Ok(token);

        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _accountService.ResetPasswordAsync(model);

            if (result.Succeeded)
            {
                return Ok("Password has been reset successfully.");
            }

            return BadRequest(result.Message);
        }

        #endregion

    }
}
