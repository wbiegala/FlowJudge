namespace FlowJudge.Common.Messaging
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class OutboxSubjectAttribute : Attribute
    {
        public OutboxSubjectAttribute(string subject)
        {
            Subject = subject;
        }

        public string Subject { get; }
    }
}
