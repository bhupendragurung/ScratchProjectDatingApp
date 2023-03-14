using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using ScratchProjectDatingApp.Entity;
using ScratchProjectDatingApp.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ScratchProjectDatingApp.Services
{
    public class TokenService : ITokenService
    {

        private readonly SymmetricSecurityKey _key;
        private readonly UserManager<AppUser> _userManager;

        public TokenService(IConfiguration config,UserManager<AppUser> userManager)
        {
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Tokenkey"]));
            _userManager = userManager;
        }
        public async  Task<string> CreateToken(AppUser user)
        {
            var claims = new List<Claim>
          {
              new Claim(JwtRegisteredClaimNames.NameId,user.Id.ToString()),
              new Claim(JwtRegisteredClaimNames.UniqueName,user.UserName),

          };
            var roles=await _userManager.GetRolesAsync(user);

            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
            var cred = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);
            var tokenDescriptor = new SecurityTokenDescriptor 
            {
                    Subject=new ClaimsIdentity(claims),
                    Expires=DateTime.Now.AddDays(7),
                    SigningCredentials=cred
            };
            var tokenhandler = new JwtSecurityTokenHandler();
            var token = tokenhandler.CreateToken(tokenDescriptor);
            return tokenhandler.WriteToken(token);
        }
    }
}
