using Application.DTOs;
using Core.Entities;
using System.Security.Claims;

namespace Application.Services
{
    public interface ITokenService
    {
        Task<List<Claim>> CreateClaimsForUserAsync(AppUser user);
        Task<LoginResult> GenerateJwtTokenAsync(IEnumerable<Claim> claims);
    }
}
