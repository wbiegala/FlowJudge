using FlowJudge.API.Service.Controllers.Mappers;
using FlowJudge.API.Service.ErrorHandling;
using FlowJudge.Common.Application.Mediator;
using FlowJudge.Users.Application.Abstractions.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace FlowJudge.API.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LegalController : ControllerBase
    {
        private readonly IMediator _mediator;

        public LegalController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("terms-and-conditions")]
        [AllowAnonymous]
        public async Task<IActionResult> GetTermsAndConditionsAsync(
            [FromQuery] Guid? version,
            CancellationToken cancellationToken = default)
        {
            if (version.HasValue)
                //TODO: implement getting specific version of document
                return StatusCode(StatusCodes.Status501NotImplemented);

            var result = await GetActualDocumentVersionAsync(Users.Application.Models.DocumentKind.TermsAndConditions, cancellationToken);

            if (!result.IsSuccess)
                return result.Error!.ToResponse(HttpStatusCode.BadRequest);

            return Ok(result.Data!.ToResponse());
        }

        [HttpGet("privacy-policy")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPrivacyPolicyAsync(
            [FromQuery] Guid? version,
            CancellationToken cancellationToken = default)
        {
            if (version.HasValue)
                //TODO: implement getting specific version of document
                return StatusCode(StatusCodes.Status501NotImplemented);

            var result = await GetActualDocumentVersionAsync(Users.Application.Models.DocumentKind.PrivacyPolicy, cancellationToken);

            if (!result.IsSuccess)
                return result.Error!.ToResponse(HttpStatusCode.BadRequest);

            return Ok(result.Data!.ToResponse());
        }

        private async Task<Common.Application.IResult<GetActualDocumentVersionQueryResult>> GetActualDocumentVersionAsync(
            Users.Application.Models.DocumentKind kind,
            CancellationToken cancellationToken)
        {
            var query = new GetActualDocumentVersionQuery
            {
                Kind = kind,
                ContentType = Users.Application.Models.DocumentContentType.Html
            };

            return await _mediator.SendQueryAsync<GetActualDocumentVersionQuery, GetActualDocumentVersionQueryResult>(query, cancellationToken);
        }
    }
}
