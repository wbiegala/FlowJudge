namespace FlowJudge.Common.Application
{
    public interface IError
    {
        string Code { get; }
        string Message { get; }
        IDictionary<string, object>? Properties { get; }
        Exception? Exception { get; }
    }
}
