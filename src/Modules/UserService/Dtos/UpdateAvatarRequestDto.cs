using Microsoft.AspNetCore.Http;

namespace UserService.Dtos
{
    public class UpdateAvatarRequestDto
    {
        public IFormFile File { get; set; } = null!;
    }
}
