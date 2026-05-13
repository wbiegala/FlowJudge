using FlowJudge.API.Contracts.Legal;
using FlowJudge.Users.Application.Abstractions.Queries;

namespace FlowJudge.API.Service.Controllers.Mappers
{
    internal static class LegalMapper
    {
        public static GetDocumentVersionResponse ToResponse(this GetActualDocumentVersionQueryResult dto) =>
            new GetDocumentVersionResponse
            {
                Kind = dto.Kind.ToString(),
                VersionId = dto.VersionId,
                VersionNumber = dto.VersionNumber,
                Content = dto.Content
            };
    }
}
