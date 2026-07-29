using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FlowJudge.Common.Messaging.Outbox.Processing
{
    internal sealed class OutboxProcessor : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly OutboxConfiguration _outboxConfiguration;
        private readonly ILogger<OutboxProcessor> _logger;

        public OutboxProcessor(
            IServiceScopeFactory scopeFactory,
            OutboxConfiguration outboxConfiguration,
            ILogger<OutboxProcessor> logger)
        {
            _scopeFactory = scopeFactory;
            _outboxConfiguration = outboxConfiguration;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var timer = new PeriodicTimer(TimeSpan.FromSeconds(_outboxConfiguration.ProcessingIntervalInSeconds));

            try
            {
                while (await timer.WaitForNextTickAsync(stoppingToken))
                {
                    await ProcessOutboxMessagesAsync(stoppingToken);
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                _logger.LogError("OutboxProcessor is stopping due to cancellation.");
            }
        }

        private async Task ProcessOutboxMessagesAsync(CancellationToken cancellationToken)
        {
            var processingId = Guid.NewGuid();
            _logger.LogDebug("Starting to process outbox messages. ProcessingId={processingId}", processingId);

            try
            {
                await using var scope = _scopeFactory.CreateAsyncScope();
                var processingService = scope.ServiceProvider.GetRequiredService<IOutboxProcessingService>();

                await processingService.ProcessBatchAsync(cancellationToken);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                _logger.LogError("Processing of outbox messages was canceled. ProcessingId={processingId}", processingId);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing outbox messages. ProcessingId={processingId}", processingId);

            }
            finally
            {
                _logger.LogDebug("Finished processing outbox messages. ProcessingId={processingId}", processingId);
            }
        }
    }
}
