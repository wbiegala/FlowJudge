namespace FlowJudge.Common.Utils.Time
{
    internal sealed class TimeService : ITimeService
    {
        public DateTimeOffset Now => DateTimeOffset.Now;

        public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
    }
}
