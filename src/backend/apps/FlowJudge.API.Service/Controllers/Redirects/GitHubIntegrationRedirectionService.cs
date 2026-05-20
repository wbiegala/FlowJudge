using FlowJudge.Common.Http.Extensions;

namespace FlowJudge.API.Service.Controllers.Redirects
{
    public class GitHubIntegrationRedirectionService
    {
        private readonly string _uiBaseUrl;

        internal GitHubIntegrationRedirectionService(string uiBaseUrl)
        {
            _uiBaseUrl = uiBaseUrl;
        }

        public string GetGitHubInstallationCallbackSuccessRedirectUrl(Guid workspaceId, Guid installationStateId)
        {
            var uri = UriBuilderExtensions.CombineUri(_uiBaseUrl, $"/w/{workspaceId}/integrations/setup/github/{installationStateId}");
            return uri.ToString();
        }
    }
}
