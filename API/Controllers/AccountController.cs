using Application.DTOs;
using Application.Services;
using Connect.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/account")]
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

        [HttpPost("register/customer")]
        public async Task<IActionResult> RegisterCustomer([FromBody] CustomerRegistrationDto customerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object>
                {
                    StatusCode = 400,
                    Message = "Validation error",
                    Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList(),
                });
            }

            var result = await _accountService.RegisterCustomerAsync(customerDto);

            if (!result.Succeeded)
            {
                return BadRequest(new ApiResponse<object>
                {
                    StatusCode = 400,
                    Message = "Customer registration failed",
                    Errors = result.Errors.Select(e => e.Description).ToList(),
                });
            }

            return Ok(new ApiResponse<object>
            {
                StatusCode = 200,
                Message = "Customer registered successfully, please check your email for OTP confirmation.",
                Data = null,
            });
        }

        [HttpPost("register/seller")]
        public async Task<IActionResult> RegisterSeller([FromBody] SellerRegistrationDto sellerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object>
                {
                    StatusCode = 400,
                    Message = "Validation error",
                    Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList(),
                });
            }

            var result = await _accountService.RegisterSellerAsync(sellerDto);

            if (!result.Succeeded)
            {
                return BadRequest(new ApiResponse<object>
                {
                    StatusCode = 400,
                    Message = "Seller registration failed",
                    Errors = result.Errors.Select(e => e.Description).ToList(),
                });
            }

            return Ok(new ApiResponse<object>
            {
                StatusCode = 200,
                Message = "Seller registered successfully, please check your email for OTP confirmation.",
                Data = null,
            });
        }

        #endregion

        #region Login

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserDto userDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object>
                {
                    StatusCode = 400,
                    Message = "Validation error",
                    Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList(),
                });
            }

            try
            {
                var result = await _accountService.Login(userDto);

                if (result.Success)
                {
                    return Ok(new ApiResponse<object>
                    {
                        StatusCode = 200,
                        Message = "Login successful",
                        Data = result,
                    });
                }

                var (errorMessage, statusCode) = result.ErrorType switch
                {
                    LoginErrorType.UserNotFound => ("User not found.", 404),
                    LoginErrorType.InvalidPassword => ("Incorrect password.", 401),
                    LoginErrorType.EmailNotConfirmed => ("Email not confirmed.", 403),
                    _ => ("Invalid login attempt.", 400)
                };

                return StatusCode(statusCode, new ApiResponse<object>
                {
                    StatusCode = statusCode,
                    Message = errorMessage,
                    Data = new { },
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during login");

                return StatusCode(500, new ApiResponse<object>
                {
                    StatusCode = 500,
                    Message = "An unexpected error occurred. Please try again later.",
                    Data = new { }
                });
            }
        }

        [HttpPost("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromBody] VerifyOtpDto verifyDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object>
                {
                    StatusCode = 400,
                    Message = "Validation error",
                    Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList(),
                });
            }

            var isConfirmed = await _accountService.ConfirmEmailWithOTP(verifyDto);
            if (isConfirmed.Succeeded)
            {
                return Ok(new ApiResponse<object>
                {
                    StatusCode = 200,
                    Message = "Email confirmed successfully.",
                    Data = null,
                });
            }
            else
            {
                return BadRequest(new ApiResponse<object>
                {
                    StatusCode = 400,
                    Message = "Invalid OTP or email confirmation failed.",
                    Data = null,
                });
            }
        }

        #endregion

        #region ChangePassword 
        [Authorize]
        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object>
                {
                    StatusCode = 400,
                    Message = "Validation error",
                    Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()
                });
            }

            var changePasswordResult = await _accountService.ChangePassword(changePasswordDto);

            if (!changePasswordResult.Succeeded)
            {
                return BadRequest(new ApiResponse<object>
                {
                    StatusCode = 400,
                    Message = "Password change failed",
                    Errors = changePasswordResult.Errors.Select(e => e.Description).ToList()
                });
            }

            return Ok(new ApiResponse<object>
            {
                StatusCode = 200,
                Message = "Password changed successfully",
                Data = null
            });
        }
        #endregion

        #region GetUsers
        [HttpGet("customer")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetCurrentCustomer()
        {
            try
            {
                var customer = await _accountService.GetCurrentCustomerAsync();
                return Ok(new ApiResponse<object>
                {
                    StatusCode = 200,
                    Message = "Customer retrieved successfully",
                    Data = customer
                });
            }
            catch (Exception ex)
            {
                return NotFound(new ApiResponse<object>
                {
                    StatusCode = 404,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        [HttpGet("seller")]
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> GetCurrentSeller()
        {
            try
            {
                var seller = await _accountService.GetCurrentSellerAsync();
                return Ok(new ApiResponse<object>
                {
                    StatusCode = 200,
                    Message = "Seller retrieved successfully",
                    Data = seller
                });
            }
            catch (Exception ex)
            {
                return NotFound(new ApiResponse<object>
                {
                    StatusCode = 404,
                    Message = ex.Message,
                    Data = null
                });
            }
        }
        #endregion

        #region ForgetPassword
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest(new ApiResponse<object>
                {
                    StatusCode = 400,
                    Message = "Email is required.",
                    Data = null
                });
            }

            var result = await _accountService.ForgotPasswordAsync(email);

            if (result.Succeeded)
            {
                return Ok(new ApiResponse<object>
                {
                    StatusCode = 200,
                    Message = "OTP has been sent to the provided email.",
                    Data = null
                });
            }

            return BadRequest(new ApiResponse<object>
            {
                StatusCode = 400,
                Message = result.Message,
                Data = null
            });
        }

        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp(string email, string otp)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(otp))
            {
                return BadRequest(new ApiResponse<object>
                {
                    StatusCode = 400,
                    Message = "Email and OTP are required.",
                    Data = null
                });
            }

            var token = await _accountService.VerifyOtpAndGenerateTokenAsync(email, otp);

            if (string.IsNullOrEmpty(token))
            {
                return BadRequest(new ApiResponse<object>
                {
                    StatusCode = 400,
                    Message = "Invalid OTP or operation failed.",
                    Data = null
                });
            }

            return Ok(new ApiResponse<object>
            {
                StatusCode = 200,
                Message = "OTP verified successfully.",
                Data = token
            });
        }


        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object>
                {
                    StatusCode = 400,
                    Message = "Validation error",
                    Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()
                });
            }

            var result = await _accountService.ResetPasswordAsync(model);

            if (result.Succeeded)
            {
                return Ok(new ApiResponse<object>
                {
                    StatusCode = 200,
                    Message = "Password has been reset successfully.",
                    Data = null
                });
            }

            return BadRequest(new ApiResponse<object>
            {
                StatusCode = 400,
                Message = "Password reset failed.",
                Data = null
            });
        }

        #endregion

        [HttpPost("add-profile-picture")]
        public async Task<IActionResult> AddProfilePicture(IFormFile file)
        {
            var response = new ApiResponse<string>();
            try
            {
                var result = await _accountService.AddProfilePictureAsync(file);

                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "Profile picture added successfully.";
                response.Data = result;
            }
            catch (Exception ex)
            {
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = "Error adding profile picture.";
                response.Errors.Add(ex.Message);
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPut("update-profile-picture")]
        public async Task<IActionResult> UpdateProfilePicture(IFormFile file)
        {
            var response = new ApiResponse<string>();
            try
            {
                var result = await _accountService.UpdateProfilePictureAsync(file);

                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "Profile picture updated successfully.";
                response.Data = result;
            }
            catch (Exception ex)
            {
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = "Error updating profile picture.";
                response.Errors.Add(ex.Message);
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpDelete("delete-profile-picture")]
        public async Task<IActionResult> DeleteProfilePicture()
        {
            var response = new ApiResponse<bool>();
            try
            {
                var result = await _accountService.DeleteProfilePictureAsync();

                response.StatusCode = StatusCodes.Status204NoContent;
                response.Message = "Profile picture deleted successfully.";
                response.Data = result;
            }
            catch (Exception ex)
            {
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = "Error deleting profile picture.";
                response.Errors.Add(ex.Message);
                return BadRequest(response);
            }

            return NoContent();
        }
    }



}

