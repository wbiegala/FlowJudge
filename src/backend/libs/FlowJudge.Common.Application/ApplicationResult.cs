namespace FlowJudge.Common.Application
{
    internal sealed record ApplicationResult : IResult
    {
        public bool IsSuccess { get; internal init; }
        public IError? Error { get; internal init; }
    }

    public sealed record ApplicationResult<TResult> : IResult<TResult>
    {
        public TResult? Data { get; internal init; }
        public bool IsSuccess { get; internal init; }
        public IError? Error { get; internal init; }
    }
}
