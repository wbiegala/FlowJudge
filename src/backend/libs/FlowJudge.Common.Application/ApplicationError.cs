namespace FlowJudge.Common.Application
{
    internal sealed record ApplicationError : IError
    {
        public required string Code { get; internal init; }
        public required string Message { get; internal init; }
        public IDictionary<string, object>? Properties { get; internal init; }
        public Exception? Exception { get; internal init; }
    }
}
