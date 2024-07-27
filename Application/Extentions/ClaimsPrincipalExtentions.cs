using System.Security.Claims;

namespace Application.Extentions
{
    public static class ClaimsPrincipalExtentions
    {
        public static string RetriveEmailFromPrincipal(this ClaimsPrincipal user)
        {
            return user?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
        }
    }
}
