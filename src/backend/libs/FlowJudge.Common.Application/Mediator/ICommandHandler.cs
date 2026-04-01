namespace FlowJudge.Common.Application.Mediator
{
    public interface ICommandHandler<TCommand>
        where TCommand : ICommand
    {
        Task<IResult> ExecuteAsync(TCommand command, CancellationToken cancellationToken = default);
    }

    public interface ICommandHandler<TCommand, TResult>
        where TCommand : ICommand<TResult>
    {
        Task<IResult<TResult>> ExecuteAsync(TCommand command, CancellationToken cancellationToken = default);
    }
}
