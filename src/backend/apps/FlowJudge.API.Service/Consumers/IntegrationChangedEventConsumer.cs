using FlowJudge.Common.Application;
using FlowJudge.Common.Messaging;
using FlowJudge.VCS.Contracts.Events;
using FlowJudge.Workspaces.Application.Abstractions.Services;

namespace FlowJudge.API.Service.Consumers
{
    public sealed class IntegrationChangedEventConsumer : IConsumer<IntegrationChangedEvent>
    {
        private readonly IGitHubIntegrationService _gitHubService;
        private readonly ILogger<IntegrationChangedEventConsumer> _logger;

        public IntegrationChangedEventConsumer(
            IGitHubIntegrationService gitHubService,
            ILogger<IntegrationChangedEventConsumer> logger)
        {
            _gitHubService = gitHubService;
            _logger = logger;
        }

        public async Task ConsumeAsync(
            IntegrationChangedEvent message,
            CancellationToken cancellationToken = default)
        {
            if (message.Provider != VCS.Contracts.Shared.IntegrationProvider.GitHub)
            {
                _logger.LogWarning("Received integration changed event for unsupported provider: {Provider}", message.Provider);
                return;
            }

            var result = message.Action switch
            {
                IntegrationChangedEvent.IntegrationAction.Deleted =>
                    await _gitHubService.DeleteIntegrationAsync(message.Integration.IntegrationId, cancellationToken),
                IntegrationChangedEvent.IntegrationAction.Deactivated =>
                    await _gitHubService.DeactivateIntegrationAsync(message.Integration.IntegrationId, cancellationToken),
                IntegrationChangedEvent.IntegrationAction.Reactivated =>
                    await _gitHubService.ActivateIntegrationAsync(message.Integration.IntegrationId, cancellationToken),
                IntegrationChangedEvent.IntegrationAction.Created =>
                    await _gitHubService.UpdateIntegrationAsync(message.Integration.IntegrationId, cancellationToken),
                IntegrationChangedEvent.IntegrationAction.Updated =>
                    await _gitHubService.UpdateIntegrationAsync(message.Integration.IntegrationId, cancellationToken),
                _ => ApplicationResultFactory.Failure("Unknown action",
                    ErrorCodeGenerator.NotAcceptable($"{nameof(IntegrationChangedEvent)}.{nameof(message.Action)}"))
            };

            if (result.IsSuccess)
            {
                _logger.LogInformation("Successfully processed messega of type '{Type}' with Id '{Id}'", nameof(IntegrationChangedEvent), message.MessageId);
            }

            _logger.LogError("Failed to process message of type '{Type}' with Id '{Id}'. Error: {Error}",
                nameof(IntegrationChangedEvent), message.MessageId, result.Error);
        }
    }
}
