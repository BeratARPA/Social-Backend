namespace NotificationService.Worker.Services
{
    public interface ISmsSender
    {
        Task SendAsync(string phoneNumber, string message);
    }
}
