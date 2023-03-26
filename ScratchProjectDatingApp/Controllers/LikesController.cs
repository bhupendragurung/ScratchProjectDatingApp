using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ScratchProjectDatingApp.DTOs;
using ScratchProjectDatingApp.Entity;
using ScratchProjectDatingApp.Extensions;
using ScratchProjectDatingApp.Helper;
using ScratchProjectDatingApp.Interfaces;

namespace ScratchProjectDatingApp.Controllers
{
   
    public class LikesController : BaseApiController
    {
        private readonly IUnitOfWork _uow;

        public LikesController(IUnitOfWork uow)
        {
            _uow = uow;
        }
        [HttpPost("{username}")]
        public async Task<ActionResult> AddLike(string username)
        {
            var sourceUserId = User.GetUserId();
            var likedUser = await _uow.UserRepository.GetUserByUsernameAsync(username);
            var sourceUser = await _uow.LikesRepository.GetUserWithLikes(sourceUserId);

            if (likedUser == null) return  NotFound();

            if (sourceUser.UserName == username) return BadRequest("You cannot like yourself");

            var userLike = await _uow.LikesRepository.GetUserLike(sourceUserId, likedUser.Id);

            if (userLike != null) return BadRequest("You already like this user");

            userLike = new UserLike
            {
                SourceUserId = sourceUserId,
                TargetUserId = likedUser.Id
            };

            sourceUser.LikedUsers.Add(userLike);

            if (await _uow.Complete()) return Ok();

            return BadRequest("Failed to like user");
        }

        [HttpGet]
        public async Task<ActionResult<PagedList<LikeDto>>> GetUserLikes([FromQuery] LikesParams likesParams)
        {
            likesParams.UserId = User.GetUserId();  
            var users = await _uow.LikesRepository.GetUserLikes(likesParams);
            Response.AddPaginationHeader(new PaginationHeader(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages));
         
            return Ok(users);
        }
    }
}
