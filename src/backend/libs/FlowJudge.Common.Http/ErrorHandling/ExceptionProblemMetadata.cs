using Microsoft.AspNetCore.Mvc;

namespace FlowJudge.Common.Http.ErrorHandling
{
    public sealed record ExceptionProblemMetadata(string ContentType, int ErrorCode, ProblemDetails ErrorBody);
}
