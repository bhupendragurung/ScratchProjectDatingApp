

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ScratchProjectDatingApp.Entity;

namespace ScratchProjectDatingApp.Controllers
{
    
    public class AdminController : BaseApiController
    {
        private readonly UserManager<AppUser> _userManager;

        public AdminController( UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }
        [Authorize(Policy ="RequireAdminRole")]
        [HttpGet("users-with-roles")]
        public async Task<ActionResult> GetusersWithRoles()
        {
            var users = await _userManager.Users.
                 OrderBy(u => u.UserName)
                 .Select(u => new
                 {
                     u.Id,
                     Username = u.UserName,
                     Roles = u.UserRoles.Select(u => u.Role.Name).ToList()
                 }).ToListAsync();
            return Ok(users);
        }
        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost("edit-roles/{username}")]
        public async Task<ActionResult> EditRoles(string username,string roles)
        {
            if(string.IsNullOrEmpty(roles)) return BadRequest("You must select at least one role");
            var selectedRoles=roles.Split(',').ToArray();
            var user=await _userManager.FindByNameAsync(username);
            if (user == null) return NotFound();

            var userRoles=await _userManager.GetRolesAsync(user);
            var result = await _userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));
            if(!result.Succeeded) return BadRequest("Failed to add roles");
             result = await _userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));

            if (!result.Succeeded) return BadRequest("Failed to remove roles");

            return Ok(await _userManager.GetRolesAsync(user));
        }
        [Authorize(Policy = "ModeratorPhotoRole")]
        [HttpGet("photos-to-moderate")]
        public ActionResult GetPhotosForModeration()
        {
            return Ok(" admin or moderator  can see this");
        }
    }
}
