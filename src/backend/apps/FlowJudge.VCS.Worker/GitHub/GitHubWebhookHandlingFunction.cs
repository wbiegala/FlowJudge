using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace FlowJudge.VCS.Worker.GitHub
{
    public class GitHubWebhookHandlingFunction
    {
        private readonly ILogger<GitHubWebhookHandlingFunction> _logger;

        public GitHubWebhookHandlingFunction(ILogger<GitHubWebhookHandlingFunction> logger)
        {
            _logger = logger;
        }

        [Function("GitHubWebhookHandlingFunction")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "github/event")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            return new OkObjectResult("Welcome to Azure Functions!");
        }
    }
}
