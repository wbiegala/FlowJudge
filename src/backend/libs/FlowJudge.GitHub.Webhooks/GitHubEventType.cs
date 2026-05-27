namespace FlowJudge.GitHub.Webhooks
{
    public enum GitHubEventType
    {
        Unknown = 0,
        Ping,
        Installation,
        InstallationRepositories,
        PullRequest,
        PullRequestReview,
        PullRequestReviewComment,
        CheckRun,
        CheckSuite,
        Push
    }
}
