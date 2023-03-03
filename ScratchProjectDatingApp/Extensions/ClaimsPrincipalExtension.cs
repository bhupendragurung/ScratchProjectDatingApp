using System.Security.Claims;

namespace ScratchProjectDatingApp.Extensions
{
    public static class ClaimsPrincipalExtension
    {
        public static   string GetUsername(this ClaimsPrincipal user)
        {
           return  user.FindFirst(ClaimTypes.Name)?.Value;
        }
        public static int GetUserId(this ClaimsPrincipal user)
        {
            return Convert.ToInt32( user.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        }
    }
}
