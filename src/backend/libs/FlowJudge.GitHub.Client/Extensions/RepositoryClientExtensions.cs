using FlowJudge.GitHub.Client.Clients;
using FlowJudge.GitHub.Client.Contract.SharedModels;

namespace FlowJudge.GitHub.Client.Extensions
{
    public static class RepositoryClientExtensions
    {
        public static async Task<IReadOnlyCollection<Repository>> GetAllInstallationRepositoriesAsync(
            this IRepositoryClient client,
            string installationId,
            CancellationToken ct)
        {
            const int pageSize = 100;
            var firstPage = await client.GetInstallationRepositoriesAsync(installationId, pageSize, 1, ct);

            if (firstPage.TotalCount <= pageSize)
                return firstPage.Repositories.ToList();

            var result = new List<Repository>();
            result.AddRange(firstPage.Repositories);

            var totalCount = firstPage.TotalCount;
            var pagesCount = (int)Math.Ceiling(decimal.Divide(totalCount, pageSize));

            for (var currentPage = 2; currentPage <= pagesCount; currentPage++)
            {
                var reposPage = await client.GetInstallationRepositoriesAsync(installationId, pageSize, currentPage, ct);
                result.AddRange(reposPage.Repositories);
            }

            return result.ToList();
        }
    }
}
