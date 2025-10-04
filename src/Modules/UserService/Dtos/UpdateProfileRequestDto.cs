namespace UserService.Dtos
{
    public class UpdateProfileRequestDto
    {
        public string Username { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Bio { get; set; } = string.Empty;
        public bool IsPrivate { get; set; }
    }
}
