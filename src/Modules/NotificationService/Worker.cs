using EventBus.Base.Abstraction;
using EventBus.IntegrationEvents;
using NotificationService.Events.Handlers;

namespace NotificationService
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
            _logger.LogInformation("NotificationService.Worker ba�lat�l�yor...");

            // Worker service sonlanana kadar �al��maya devam et
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("NotificationService.Worker durduruluyor...");

            // Event subscription'lar� temizle
            try
            {
                _eventBus.Unsubscribe<SendVerificationCodeIntegrationEvent, SendVerificationCodeEventHandler>();
                _logger.LogInformation("Event subscription'lar� temizlendi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Event subscription temizlenirken hata olu�tu");
            }

            await base.StopAsync(stoppingToken);
        }
    }
}
