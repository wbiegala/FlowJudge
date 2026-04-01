namespace FlowJudge.Common.Application
{
    public interface IResult
    {
        bool IsSuccess { get; }
        IError? Error { get; }
    }

    public interface IResult<TResult> : IResult
    {
        TResult? Data { get; }
    }
}
