using Core.Entities.Identity;

namespace Application.Services
{
    public interface ITokenService
    {
        string CreateToken(AppUser user);
    }
}
