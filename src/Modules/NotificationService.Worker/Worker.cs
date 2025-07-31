using EventBus.Base.Abstraction;
using EventBus.IntegrationEvents.Verification;
using NotificationService.Worker.Events.Handlers;

namespace NotificationService.Worker
{
    public class Worker : BackgroundService
    {
        private readonly IEventBus _eventBus;
        private readonly ILogger<Worker> _logger;

        public Worker(IEventBus eventBus, ILogger<Worker> logger)
        {
            _eventBus = eventBus;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("NotificationService.Worker baþlatýlýyor...");

            // Event subscription - Bu satýr çok önemli
            _eventBus.Subscribe<SendVerificationCodeIntegrationEvent, SendVerificationCodeEventHandler>();

            _logger.LogInformation("SendVerificationCodeIntegrationEvent event'ine subscribe oldu");

            // Worker service sonlanana kadar çalýþmaya devam et
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken); // 10 saniye bekle
                _logger.LogInformation("NotificationService.Worker çalýþýyor... {time}", DateTimeOffset.Now);
            }
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("NotificationService.Worker durduruluyor...");

            // Event subscription'larý temizle
            try
            {
                _eventBus.Unsubscribe<SendVerificationCodeIntegrationEvent, SendVerificationCodeEventHandler>();
                _logger.LogInformation("Event subscription'larý temizlendi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Event subscription temizlenirken hata oluþtu");
            }

            await base.StopAsync(stoppingToken);
        }
    }
}
