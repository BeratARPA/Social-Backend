namespace NotificationService.Services
{
    public interface IWhatsAppSender
    {
        Task SendAsync(string phoneNumber, string message);
    }
}
