using FlowJudge.GitHub.Webhooks;
using FlowJudge.GitHub.Webhooks.Security;
using FlowJudge.VCS.Worker.GitHub.WebhooksServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace FlowJudge.VCS.Worker.GitHub
{
    public class GitHubWebhookHandlingFunction
    {
        private readonly IGitHubWebhookSignatureValidator _validator;
        private readonly IEnumerable<IGitHubWebhookHandler> _webhookHandlers;
        private readonly ILogger<GitHubWebhookHandlingFunction> _logger;

        public GitHubWebhookHandlingFunction(
            IGitHubWebhookSignatureValidator validator,
            IEnumerable<IGitHubWebhookHandler> webhookHandlers,
            ILogger<GitHubWebhookHandlingFunction> logger)
        {
            _validator = validator;
            _webhookHandlers = webhookHandlers;
            _logger = logger;
        }

        [Function("GitHubWebhookHandlingFunction")]
        public async Task<IActionResult> RunAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "github/event")] HttpRequest req,
            CancellationToken cancellationToken)
        {
            var requestMetadata = await GitHubWebhookMetadataExtractor.ExtractAsync(req, cancellationToken);
            if (requestMetadata is null)
                return new BadRequestResult();

            if (!_validator.IsSignatureValid(requestMetadata))
                return new UnauthorizedResult();

            var handler = _webhookHandlers.FirstOrDefault(h => h.CanHandle(requestMetadata.Type));
            if (handler is null)
            {
                _logger.LogWarning("No webhook handler for webhook type {type}", requestMetadata.Type.ToString());
                return new NoContentResult();
            }
                
            await handler.HandleAsync(requestMetadata, cancellationToken);

            return new NoContentResult();
        }
    }
}
