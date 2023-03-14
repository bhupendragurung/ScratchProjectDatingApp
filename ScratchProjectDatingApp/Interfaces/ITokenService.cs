using ScratchProjectDatingApp.Entity;

namespace ScratchProjectDatingApp.Interfaces
{
    public interface ITokenService
    {
      Task<string> CreateToken(AppUser user);
    }
}
