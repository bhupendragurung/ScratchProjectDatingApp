using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ScratchProjectDatingApp.Data;
using ScratchProjectDatingApp.DTOs;
using ScratchProjectDatingApp.Entity;
using ScratchProjectDatingApp.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace ScratchProjectDatingApp.Controllers
{
    public class AccountController : BaseApiController
    {
        
        private readonly UserManager<AppUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;

        public AccountController(UserManager<AppUser> userManager,ITokenService tokenService, IMapper mapper)
        {
          
            _userManager = userManager;
            _tokenService = tokenService;
            _mapper = mapper;
        }

        [HttpPost("register")] // Post api/account/register
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            if (await UserExists(registerDto.Username)) return BadRequest("User already taken");
            var user = _mapper.Map<AppUser>(registerDto);
            

            user.UserName = registerDto.Username.ToLower();
           
            var result =await _userManager.CreateAsync(user,registerDto.Password);
            if (!result.Succeeded) return BadRequest(result.Errors);

            var roleResult = await _userManager.AddToRoleAsync(user, "Member");
            if (!roleResult.Succeeded) return BadRequest(result.Errors);

            return new UserDto
            { Username=user.UserName,
            Token= await _tokenService.CreateToken(user),
            knownAs=user.KnownAs,
            Gender=user.Gender,
            };
        }

        [HttpPost("login")]
        public async Task<ActionResult< UserDto>> Login(LoginDto loginDto)
            {
            var user = await _userManager.Users
                .Include(x => x.Photos)
                .SingleOrDefaultAsync(x => x.UserName == loginDto.Username);
            if (user == null) return Unauthorized("Invalid Username");

            var result = await _userManager.CheckPasswordAsync(user, loginDto.Password);

            if(!result) return  Unauthorized("Invalid Password");


            return new UserDto
            {
                Username = user.UserName,
                Token = await _tokenService.CreateToken(user),
                PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url,
                knownAs = user.KnownAs,
                Gender= user.Gender, 
            };

        }
        public async Task<bool> UserExists(string username)
        {
            return await _userManager.Users.AnyAsync(x => x.UserName == username.ToLower());
        }
    }
}
