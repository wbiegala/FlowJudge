using FlowJudge.Common.Sql.UnitOfWork;
using FlowJudge.Reviews.Application.Abstractions.Ports;

namespace FlowJudge.Reviews.Infrastructure.Synchronization
{
    internal sealed class DomainSynchronizer : DapperRepository, IDomainSynchronizer
    {
        public DomainSynchronizer(ISqlSession sqlSession) : base(sqlSession)
        {
        }

        public async Task SynchronizeIntegrationAsync(CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public async Task SynchronizeRepositoryAsync(CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public async Task SynchronizeWorkspaceAsync(CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }
    }
}
