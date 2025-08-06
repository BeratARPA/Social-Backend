using Microsoft.AspNetCore.Mvc;
using UserService.Commands.DeleteAccount;
using UserService.Commands.Following;
using UserService.Commands.RestoreAccount;
using UserService.Commands.UpdateProfile;
using UserService.Dtos;
using UserService.Queries.Following;
using UserService.Queries.GetSuggestions;
using UserService.Queries.GetUser;
using UserService.Queries.SearchUsers;

namespace Social.API.Controllers
{
    public class UserController : BaseController
    {
        /// <summary>
        /// Kendi profilimi getir
        /// </summary>
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var query = new GetCurrentUserQuery(GetUserId());
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Başka kullanıcının profilini getir
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            var query = new GetUserByIdQuery(id);
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Profil bilgilerini güncelle
        /// </summary>
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequestDto request)
        {
            var command = new UpdateProfileCommand(GetUserId(), request);
            var result = await Mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// Profil fotoğrafı yükle
        /// </summary>
        [HttpPost("upload-avatar")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadAvatar([FromForm] UpdateAvatarRequestDto request)
        {
            var command = new UploadAvatarCommand(GetUserId(), request.File);
            var result = await Mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// Hesabı sil
        /// </summary>
        [HttpDelete("delete-account")]
        public async Task<IActionResult> DeleteAccount()
        {
            var command = new DeleteAccountCommand(GetUserId());
            var result = await Mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// Silinmiş hesabı geri yükle (30 gün içinde)
        /// </summary>
        [HttpPost("restore-account")]
        public async Task<IActionResult> RestoreAccount()
        {
            var command = new RestoreAccountCommand(GetUserId());
            var result = await Mediator.Send(command);
            return Ok(result);
        }

        ///// <summary>
        ///// Kullanıcı ara
        ///// </summary>
        //[HttpGet("search")]
        //public async Task<IActionResult> SearchUsers([FromQuery] string q)
        //{
        //    var query = new SearchUsersQuery(q);
        //    var result = await Mediator.Send(query);
        //    return Ok(result);
        //}

        ///// <summary>
        ///// Takip önerileri
        ///// </summary>
        //[HttpGet("suggestions")]
        //public async Task<IActionResult> GetSuggestions()
        //{
        //    var query = new GetSuggestionsQuery(GetUserId());
        //    var result = await Mediator.Send(query);
        //    return Ok(result);
        //}

        /// <summary>
        /// Kullanıcıyı takip et
        /// </summary>
        [HttpPost("{id}/follow")]
        public async Task<IActionResult> Follow(Guid id)
        {
            var command = new FollowUserCommand(GetUserId(), id);
            var result = await Mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// Kullanıcıyı takipten çıkar
        /// </summary>
        [HttpDelete("{id}/unfollow")]
        public async Task<IActionResult> Unfollow(Guid id)
        {
            var command = new UnfollowUserCommand(GetUserId(), id);
            var result = await Mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// Takipçileri listele
        /// </summary>
        [HttpGet("{id}/followers")]
        public async Task<IActionResult> GetFollowers(string id)
        {
            var query = new GetFollowersQuery(GetUserId());
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Takip ettiklerini listele
        /// </summary>
        [HttpGet("{id}/following")]
        public async Task<IActionResult> GetFollowing(string id)
        {
            var query = new GetFollowingQuery(GetUserId());
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Takip isteklerini getir (private hesaplar için)
        /// </summary>
        [HttpGet("follow-requests")]
        public async Task<IActionResult> GetFollowRequests()
        {
            var query = new GetFollowRequestsQuery(GetUserId());
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Takip isteğini kabul et
        /// </summary>
        [HttpPost("follow-requests/{id}/accept")]
        public async Task<IActionResult> AcceptFollowRequest(Guid id)
        {
            var command = new AcceptFollowRequestCommand(GetUserId(), id);
            var result = await Mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// Takip isteğini reddet
        /// </summary>
        [HttpDelete("follow-requests/{id}/decline")]
        public async Task<IActionResult> DeclineFollowRequest(Guid id)
        {
            var command = new DeclineFollowRequestCommand(GetUserId(), id);
            var result = await Mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// İki kullanıcı arasındaki takip durumunu getir
        /// </summary>
        [HttpGet("{targetUserId}/follow-status")]
        public async Task<IActionResult> GetFollowStatus(Guid targetUserId)
        {
            var query = new GetFollowStatusQuery(GetUserId(), targetUserId);
            var result = await Mediator.Send(query);
            return Ok(result);
        }
    }
}
