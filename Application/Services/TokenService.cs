using Application.DTOs;
using Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Application.Services
{
    public class TokenService : ITokenService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IConfiguration _config;

        public TokenService(UserManager<AppUser> userManager, IConfiguration config)
        {

            _userManager = userManager;
            _config = config;

        }


        #region JWT
        public async Task<List<Claim>> CreateClaimsForUserAsync(AppUser user)
        {
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, user.UserName),
        new Claim(ClaimTypes.Email, user.Email),
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
    };

            var roles = _userManager.GetRolesAsync(user).Result;
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            return claims;
        }

        public async Task<LoginResult> GenerateJwtTokenAsync(IEnumerable<Claim> claims)
        {
            var jwtConfig = _config.GetSection("JWT");
            var secretKey = jwtConfig["Secret"];
            var validIssuer = jwtConfig["ValidIssuer"];
            var validAudience = jwtConfig["ValidAudience"];

            if (string.IsNullOrEmpty(secretKey) || string.IsNullOrEmpty(validIssuer) || string.IsNullOrEmpty(validAudience))
            {
                throw new InvalidOperationException("JWT configuration is not set properly.");
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var tokenExpiration = DateTime.UtcNow.AddDays(1); // Consider using a configuration value for expiration time
            var token = new JwtSecurityToken(
                issuer: validIssuer,
                audience: validAudience,
                claims: claims,
                expires: tokenExpiration,
                signingCredentials: signingCredentials
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return new LoginResult
            {
                Success = true,
                Token = tokenString,
                Expiration = token.ValidTo
            };
        }
        #endregion

    }
}
