using FlowJudge.Common.Sql.UnitOfWork;

namespace FlowJudge.Common.Application.Transactional
{
    public abstract class TransactionalService
    {
        private readonly IUnitOfWork _unitOfWork;

        public TransactionalService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        protected async Task RunInTransactionAsync(Func<Task> action, CancellationToken cancellationToken = default)
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                await action();
                await _unitOfWork.CommitAsync(cancellationToken);
            }
            catch
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                throw;
            }
        }

        protected async Task<T> RunInTransactionAsync<T>(Func<Task<T>> action, CancellationToken cancellationToken = default)
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                var result = await action();
                await _unitOfWork.CommitAsync(cancellationToken);
                return result;
            }
            catch
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                throw;
            }
        }
    }
}
