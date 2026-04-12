using FlowJudge.Common.Domain;

namespace FlowJudge.Users.Domain.Model.Exceptions
{
    internal sealed class DocumentVersionNumberException : DomainException
    {
        public DocumentVersionNumberException(string errorCode)
            : base(Domain.BoundedContext.Name, nameof(User), errorCode) { }
    }
}
