using Microsoft.AspNetCore.Identity;

namespace ScratchProjectDatingApp.Entity
{
    public class AppRole:IdentityRole<int>
    {
        public ICollection<AppUserRole> UserRoles { get; set; }
    }
}
