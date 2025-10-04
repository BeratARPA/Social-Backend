using Microsoft.AspNetCore.Mvc;
using UserService.Commands.UpdateProfile;
using UserService.Dtos;
using UserService.Queries.GetUser;

namespace Social.API.Controllers
{
    public class UserController : BaseController
    {
        [HttpGet("GetCurrentProfile")]
        public async Task<IActionResult> GetCurrentProfile()
        {
            var query = new GetUserByUserIdQuery(GetUserId());
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("GetProfile")]
        public async Task<IActionResult> GetProfileByUserId(Guid userId)
        {
            var query = new GetUserByUserIdQuery(userId);
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        [HttpPut("UpdateProfile")]
        public async Task<IActionResult> UpdateProfile(Guid userId, [FromBody] UpdateProfileRequestDto request)
        {
            var command = new UpdateProfileCommand(userId, request);
            var result = await Mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("upload-avatar")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadAvatar([FromForm] UpdateAvatarRequestDto request)
        {
            var command = new UploadAvatarCommand(GetUserId(), request.File);
            var result = await Mediator.Send(command);
            return Ok(result);
        }
    }
}
