namespace NotificationService.Services
{
    public class SmsSender : ISmsSender
    {
        private readonly ILogger<SmsSender> _logger;

        public SmsSender(ILogger<SmsSender> logger)
        {
            _logger = logger;
        }

        public Task SendAsync(string phoneNumber, string message)
        {
            _logger.LogInformation($"[SmsSender] SMS gönderiliyor => To: {phoneNumber}, Message: {message}");
            return Task.CompletedTask;
        }
    }
}
