using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FlowJudge.Common.Messaging.Consumption
{
    internal sealed class MessageProcessingService : BackgroundService
    {
        private readonly ServiceBusClient _serviceBusClient;
        private readonly MessageDispatcher _dispatcher;
        private readonly IReadOnlyCollection<ConsumerOptions> _options;
        private readonly ILogger<MessageProcessingService> _logger;
        private readonly List<ServiceBusProcessor> _processors = new();

        public MessageProcessingService(
            ServiceBusClient serviceBusClient,
            MessageDispatcher dispatcher,
            IEnumerable<ConsumerOptions> options,
            ILogger<MessageProcessingService> logger)
        {
            _serviceBusClient = serviceBusClient;
            _dispatcher = dispatcher;
            _options = options.ToArray();
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            foreach (var options in _options)
            {
                var processorOptions = new ServiceBusProcessorOptions
                {
                    AutoCompleteMessages = options.AutoCompleteMessages,
                    MaxConcurrentCalls = options.MaxConcurrentCalls,
                    MaxAutoLockRenewalDuration = TimeSpan.FromSeconds(options.MaxAutoLockRenewalDurationSeconds),
                    ReceiveMode = ServiceBusReceiveMode.PeekLock
                };

                var processor = options switch
                {
                    QueueConsumerOptions queueOptions => _serviceBusClient.CreateProcessor(
                        queueOptions.QueueName,
                        processorOptions),
                    SubscriptionConsumerOptions subsctiptionOptions => _serviceBusClient.CreateProcessor(
                        subsctiptionOptions.TopicName,
                        subsctiptionOptions.SubscriptionName,
                        processorOptions),
                    _ => throw new InvalidOperationException(
                        $"Unsupported consumer options type '{options.GetType().FullName}'.")
                };

                processor.ProcessMessageAsync += args => ProcessMessageAsync(args, options);
                processor.ProcessErrorAsync += ProcessErrorAsync;

                _processors.Add(processor);
            }

            var processorsStarter = _processors.Select(p => p.StartProcessingAsync(stoppingToken));

            await Task.WhenAll(processorsStarter);

            try
            {
                await Task.Delay(Timeout.InfiniteTimeSpan, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
            }
        }

        private async Task ProcessMessageAsync(ProcessMessageEventArgs args, ConsumerOptions options)
        {
            try
            {
                await _dispatcher.DispatchAsync(args.Message, options, args.CancellationToken);

                if (!options.AutoCompleteMessages)
                {
                    await args.CompleteMessageAsync(args.Message, args.CancellationToken);
                }
            }
            catch (UnsupportedIntegrationEventException exception)
            {
                _logger.LogError(
                    exception,
                    "Unsupported integration event. MessageId: {MessageId}.",
                    args.Message.MessageId);

                await args.DeadLetterMessageAsync(
                    args.Message,
                    deadLetterReason: "UnsupportedIntegrationEvent",
                    deadLetterErrorDescription: exception.Message,
                    cancellationToken: args.CancellationToken);
            }
            catch (Exception exception)
            {
                _logger.LogError(
                    exception,
                    "Error while processing Service Bus message. MessageId: {MessageId}.",
                    args.Message.MessageId);

                throw;
            }
        }

        private Task ProcessErrorAsync(ProcessErrorEventArgs args)
        {
            _logger.LogError(
                args.Exception,
                "Service Bus processor error. EntityPath: {EntityPath}, ErrorSource: {ErrorSource}.",
                args.EntityPath,
                args.ErrorSource);

            return Task.CompletedTask;
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            foreach (var processor in _processors)
            {
                await processor.StopProcessingAsync(cancellationToken);
                await processor.DisposeAsync();
            }

            await base.StopAsync(cancellationToken);
        }
    }
}
