using System.Security.Claims;

namespace ScratchProjectDatingApp.Extensions
{
    public static class ClaimsPrincipalExtension
    {
        public static   string GetUsername(this ClaimsPrincipal user)
        {
           return  user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
    }
}
