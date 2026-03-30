namespace FlowJudge.Common.Application.Mediator
{
    public interface IQueryHandler<TQuery, TResult>
        where TQuery : IQuery<TResult>
    {
        Task<IResult<TResult>> HandleAsync(TQuery query, CancellationToken cancellationToken = default);
    }
}
