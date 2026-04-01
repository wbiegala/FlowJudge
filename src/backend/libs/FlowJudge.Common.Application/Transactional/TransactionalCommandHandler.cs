using FlowJudge.Common.Application.Mediator;
using FlowJudge.Common.Sql.UnitOfWork;

namespace FlowJudge.Common.Application.Transactional
{
    public abstract class TransactionalCommandHandler<TCommand> : ICommandHandler<TCommand>
        where TCommand : ICommand
    {
        private readonly IUnitOfWork _unitOfWork;

        protected TransactionalCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IResult> ExecuteAsync(TCommand command, CancellationToken cancellationToken = default)
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                var result = await ExecuteInTransactionAsync(command, cancellationToken);
                await _unitOfWork.CommitAsync(cancellationToken);

                return result;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                return ApplicationResultFactory.Failure(ex, ex.GetType().Name);
            }
        }

        protected abstract Task<IResult> ExecuteInTransactionAsync(
            TCommand command,
            CancellationToken cancellationToken = default);
    }

    public abstract class TransactionalCommandHandler<TCommand, TResult> : ICommandHandler<TCommand, TResult>
        where TCommand : ICommand<TResult>
    {
        private readonly IUnitOfWork _unitOfWork;

        protected TransactionalCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IResult<TResult>> ExecuteAsync(TCommand command, CancellationToken cancellationToken = default)
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                var result = await ExecuteInTransactionAsync(command, cancellationToken);
                await _unitOfWork.CommitAsync(cancellationToken);

                return result;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                return ApplicationResultFactory.Failure<TResult>(ex, ex.GetType().Name);
            }
        }

        protected abstract Task<IResult<TResult>> ExecuteInTransactionAsync(
            TCommand command,
            CancellationToken cancellationToken = default);
    }
}
