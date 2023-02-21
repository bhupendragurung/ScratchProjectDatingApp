using ScratchProjectDatingApp.Entity;

namespace ScratchProjectDatingApp.Interfaces
{
    public interface ITokenService
    {
      string CreateToken(AppUser user);
    }
}
