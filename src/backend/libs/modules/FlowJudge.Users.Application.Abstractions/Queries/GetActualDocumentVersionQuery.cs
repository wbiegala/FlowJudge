using FlowJudge.Common.Application.Mediator;
using FlowJudge.Users.Application.Models;

namespace FlowJudge.Users.Application.Abstractions.Queries
{
    public sealed record GetActualDocumentVersionQuery : IQuery<GetActualDocumentVersionQueryResult>
    {
        public DocumentKind Kind { get; init; }
        public DocumentContentType ContentType { get; init; }
    }
}
