namespace FlowJudge.Common.Utils.Time
{
    public interface ITimeService
    {
        DateTimeOffset Now { get; }
        DateTimeOffset UtcNow { get; }
    }
}
