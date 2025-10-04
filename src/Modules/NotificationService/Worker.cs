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
            _logger.LogInformation("NotificationService.Worker baþlatýlýyor...");

            // Worker service sonlanana kadar çalýþmaya devam et
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
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
