using EventBus.Base.Abstraction;
using EventBus.IntegrationEvents.Verification;
using NotificationService.Worker.Events.Handlers;

namespace NotificationService.Worker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IEventBus _eventBus;

        public Worker(ILogger<Worker> logger, IEventBus eventBus)
        {
            _logger = logger;
            _eventBus = eventBus;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Worker baþlatýldý: {time}", DateTimeOffset.Now);

            // EventBus subscribe
            _eventBus.Subscribe<SendVerificationCodeIntegrationEvent, SendVerificationCodeEventHandler>();

            return Task.CompletedTask;
        }
    }
}
