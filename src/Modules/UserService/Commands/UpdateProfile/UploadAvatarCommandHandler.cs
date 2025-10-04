using ExceptionHandling.Exceptions;
using MediatR;
using UserService.Data.Entities;
using UserService.Data.Repositories;

namespace UserService.Commands.UpdateProfile
{
    public class UploadAvatarCommandHandler : IRequestHandler<UploadAvatarCommand, string>
    {
        private readonly IGenericRepository<UserProfile> _userProfileRepository;

        public UploadAvatarCommandHandler(
            IGenericRepository<UserProfile> userProfileRepository)
        {
            _userProfileRepository = userProfileRepository;
        }

        public async Task<string> Handle(UploadAvatarCommand request, CancellationToken cancellationToken)
        {
            var user = await _userProfileRepository.FirstOrDefaultAsync(x => x.UserId == request.UserId);

            if (user == null)
                throw new NotFoundException("UserNotFound");

            if (request.File.Length > 2 * 1024 * 1024) // 2 MB
                throw new ValidationException("Dosya boyutu 2 MB'den büyük olamaz.");

            var allowedTypes = new[] { "image/jpeg", "image/png", "image/webp" };
            if (!allowedTypes.Contains(request.File.ContentType))
                throw new ValidationException("Geçersiz dosya türü.");

            // Ana klasör: wwwroot/Contents
            var rootFolder = Path.Combine(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), "Contents");

            // Kullanıcı klasörü: wwwroot/SContents/{UserId}
            var userFolder = Path.Combine(rootFolder, user.UserId.ToString());

            // Avatar klasörü: wwwroot/Contents/{UserId}/Avatars
            var avatarFolder = Path.Combine(userFolder, "Avatars");

            // Klasörleri oluştur
            if (!Directory.Exists(avatarFolder))
                Directory.CreateDirectory(avatarFolder);

            // Dosya ismi: avatar.jpg/png...
            var fileExtension = Path.GetExtension(request.File.FileName);
            var fileName = $"{user.UserId}_{request.File.FileName}{fileExtension}";

            var filePath = Path.Combine(avatarFolder, fileName);

            // Kopyalama
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await request.File.CopyToAsync(stream, cancellationToken);
            }

            // Avatar URL kaydet (örnek: /Contents/{UserId}/Avatars/avatar.jpg)
            user.AvatarUrl = $"/Contents/{user.UserId}/Avatars/{fileName}";

            await _userProfileRepository.UpdateAsync(user);
            await _userProfileRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

            return user.AvatarUrl;
        }
    }
}
