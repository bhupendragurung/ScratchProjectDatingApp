using Microsoft.AspNetCore.Identity;

namespace ScratchProjectDatingApp.Entity
{
    public class AppUserRole:IdentityUserRole<int>
    {
        public AppUser User { get; set; }
        public AppRole Role { get; set; }
    }
}
