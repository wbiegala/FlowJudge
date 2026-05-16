using FlowJudge.Common.Utils.Time;
using FlowJudge.Workspaces.Domain.Integration.Model;
using FlowJudge.Workspaces.Domain.Workspace.Model;

namespace FlowJudge.Workspaces.Domain.Integration.Services.Impl
{
    internal sealed class IntegrationFactory : IIntegrationFactory
    {
        private readonly ITimeService _timeService;

        public IntegrationFactory(ITimeService timeService)
        {
            _timeService = timeService;
        }

        public GithubIntegration CreateGithubIntegration(
            WorkspaceId workspaceId,
            IntegrationName name,
            Guid creatorId)
        {
            return new GithubIntegration(workspaceId, name, _timeService.Now, creatorId);
        }
    }
}
