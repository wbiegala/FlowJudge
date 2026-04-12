namespace FlowJudge.Common.Domain
{
    public abstract class DomainException : Exception
    {
        public string BoundedContext { get; }
        public string DomainModelName { get; }
        public string ErrorCode { get; }

        protected DomainException(string boundedContext, string domainModelName, string errorCode)
        {
            BoundedContext = boundedContext;
            DomainModelName = domainModelName;
            ErrorCode = errorCode;
        }
    }
}
