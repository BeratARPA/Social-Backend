using Microsoft.AspNetCore.Mvc;

namespace Social.API.Controllers
{
    public class PostController : BaseController
    {
        ///// <summary>
        ///// Ana sayfa feed'ini getir
        ///// </summary>
        //[HttpGet("feed")]
        //public async Task<ActionResult> GetFeed([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        //{
        //    return Ok();
        //}

        ///// <summary>
        ///// Keşfet sayfası - trend ve önerilen postlar
        ///// </summary>
        //[HttpGet("explore")]
        //public async Task<ActionResult> GetExplorePosts([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        //{
        //    return Ok();
        //}

        ///// <summary>
        ///// Yeni post oluştur
        ///// </summary>
        //[HttpPost]
        //public async Task<ActionResult> CreatePost([FromBody] CreatePostRequest request)
        //{
        //    return Ok();
        //}

        ///// <summary>
        ///// Medya ile post oluştur
        ///// </summary>
        //[HttpPost("with-media")]
        //public async Task<ActionResult> CreatePostWithMedia([FromForm] CreatePostWithMediaRequest request)
        //{
        //    return Ok();
        //}

        ///// <summary>
        ///// Tek post getir
        ///// </summary>
        //[HttpGet("{id}")]
        //public async Task<ActionResult> GetPost(string id)
        //{
        //    return Ok();
        //}

        ///// <summary>
        ///// Post düzenle
        ///// </summary>
        //[HttpPut("{id}")]
        //public async Task<ActionResult> UpdatePost(string id, [FromBody] UpdatePostRequest request)
        //{
        //    return Ok();
        //}

        ///// <summary>
        ///// Post sil
        ///// </summary>
        //[HttpDelete("{id}")]
        //public async Task<ActionResult> DeletePost(string id)
        //{
        //    return Ok();
        //}

        ///// <summary>
        ///// Belirli kullanıcının postlarını getir
        ///// </summary>
        //[HttpGet("user/{userId}")]
        //public async Task<ActionResult> GetUserPosts(string userId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        //{
        //    return Ok();
        //}

        ///// <summary>
        ///// Post beğen
        ///// </summary>
        //[HttpPost("{id}/like")]
        //public async Task<ActionResult> LikePost(string id)
        //{
        //    return Ok();
        //}

        ///// <summary>
        ///// Post beğenisini kaldır
        ///// </summary>
        //[HttpDelete("{id}/unlike")]
        //public async Task<ActionResult> UnlikePost(string id)
        //{
        //    return Ok();
        //}

        ///// <summary>
        ///// Post beğenenlerini listele
        ///// </summary>
        //[HttpGet("{id}/likes")]
        //public async Task<ActionResult> GetPostLikes(string id, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        //{
        //    return Ok();
        //}

        ///// <summary>
        ///// Posta yorum yap
        ///// </summary>
        //[HttpPost("{id}/comments")]
        //public async Task<ActionResult> AddComment(string id, [FromBody] CreateCommentRequest request)
        //{
        //    return Ok();
        //}

        ///// <summary>
        ///// Post yorumlarını getir
        ///// </summary>
        //[HttpGet("{id}/comments")]
        //public async Task<ActionResult> GetPostComments(string id, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        //{
        //    return Ok();
        //}

        ///// <summary>
        ///// Yorumu düzenle
        ///// </summary>
        //[HttpPut("comments/{commentId}")]
        //public async Task<ActionResult> UpdateComment(string commentId, [FromBody] UpdateCommentRequest request)
        //{
        //    return Ok();
        //}

        ///// <summary>
        ///// Yorumu sil
        ///// </summary>
        //[HttpDelete("comments/{commentId}")]
        //public async Task<ActionResult> DeleteComment(string commentId)
        //{
        //    return Ok();
        //}

        ///// <summary>
        ///// Yorumu beğen
        ///// </summary>
        //[HttpPost("comments/{commentId}/like")]
        //public async Task<ActionResult> LikeComment(string commentId)
        //{
        //    return Ok();
        //}

        ///// <summary>
        ///// Yoruma cevap ver
        ///// </summary>
        //[HttpPost("comments/{commentId}/reply")]
        //public async Task<ActionResult> ReplyToComment(string commentId, [FromBody] CreateCommentRequest request)
        //{
        //    return Ok();
        //}

        ///// <summary>
        ///// Hashtag ile post ara
        ///// </summary>
        //[HttpGet("hashtag/{hashtag}")]
        //public async Task<ActionResult> GetPostsByHashtag(string hashtag, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        //{
        //    return Ok();
        //}

        ///// <summary>
        ///// Postu kaydet (bookmark)
        ///// </summary>
        //[HttpPost("{id}/save")]
        //public async Task<ActionResult> SavePost(string id)
        //{
        //    return Ok();
        //}

        ///// <summary>
        ///// Kaydedilen postu kaldır
        ///// </summary>
        //[HttpDelete("{id}/unsave")]
        //public async Task<ActionResult> UnsavePost(string id)
        //{
        //    return Ok();
        //}

        ///// <summary>
        ///// Kaydedilen postları getir
        ///// </summary>
        //[HttpGet("saved")]
        //public async Task<ActionResult> GetSavedPosts([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        //{
        //    return Ok();
        //}
    }
}
