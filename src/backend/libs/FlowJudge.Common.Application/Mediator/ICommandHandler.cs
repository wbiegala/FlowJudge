namespace FlowJudge.Common.Application.Mediator
{
    public interface ICommandHandler<TCommand>
        where TCommand : ICommand
    {
        Task<IResult> ExecuteAsync(TCommand command, CancellationToken cancellationToken = default);
    }
}
