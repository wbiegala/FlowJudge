using FlowJudge.GitHub.Webhooks;
using FlowJudge.GitHub.Webhooks.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace FlowJudge.VCS.Worker.GitHub
{
    public class GitHubWebhookHandlingFunction
    {
        private readonly IGitHubWebhookSignatureValidator _validator;
        private readonly ILogger<GitHubWebhookHandlingFunction> _logger;

        public GitHubWebhookHandlingFunction(
            IGitHubWebhookSignatureValidator validator,
            ILogger<GitHubWebhookHandlingFunction> logger)
        {
            _validator = validator;
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

            _logger.LogInformation("C# HTTP trigger function processed a request.");
            return new OkObjectResult("Welcome to Azure Functions!");
        }
    }
}
