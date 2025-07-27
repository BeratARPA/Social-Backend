namespace AuthService.Dtos
{
    public class ConfirmEmailRequestDto
    {
        public string Email { get; set; }
        public string Code { get; set; }
    }
}
