using FlowJudge.API.Contracts.User;
using FlowJudge.API.Service.ErrorHandling;
using FlowJudge.Common.Application.Mediator;
using FlowJudge.Common.Http.Extensions;
using FlowJudge.Users.Application.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlowJudge.API.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UserController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPut("terms-and-conditions/{termsVersionId:guid}/accept")]
        [Authorize]
        public async Task<IActionResult> AcceptTermsAndConditionsAsync(
            [FromRoute] Guid termsVersionId,
            CancellationToken cancellationToken = default)
        {
            var userContext = this.HttpContext.User.GetUserContext();
            var command = new Users.Application.Commands.AcceptTermsAndConditionsCommand(userContext.Id, termsVersionId);
            var result = await _mediator.SendCommandAsync(command, cancellationToken);

            if (!result.IsSuccess)
            {
                return BadRequest(result.Error!.ToProblemDetails());
            }

            return Ok();
        }

        [HttpPut("privacy-policy/{privacyPolicyVersionId:guid}/accept")]
        [Authorize]
        public async Task<IActionResult> AcceptPrivacyPolicyAsync(
            [FromRoute] Guid privacyPolicyVersionId,
            CancellationToken cancellationToken = default)
        {
            var userContext = this.HttpContext.User.GetUserContext();
            var command = new Users.Application.Commands.AcceptPrivacyPolicyCommand(userContext.Id, privacyPolicyVersionId);
            var result = await _mediator.SendCommandAsync(command, cancellationToken);

            if (!result.IsSuccess)
            {
                return BadRequest(result.Error!.ToProblemDetails());
            }

            return Ok();
        }

        [HttpGet("legal-state")]
        [Authorize]
        public async Task<IActionResult> GetUserLegalStateAsync(CancellationToken cancellationToken = default)
        {
            var userContext = this.HttpContext.User.GetUserContext();
            var query = new GetUserLegalStateQuery(userContext.Id);
            var result = await _mediator.SendQueryAsync<GetUserLegalStateQuery, GetUserLegalStateQueryResult>(query, cancellationToken);

            if (!result.IsSuccess)
            {
                return BadRequest(result.Error!.ToProblemDetails());
            }

            return Ok(new GetUserLegalStateResponse { IsLegal = result.Data!.IsValid, Missings = result.Data?.Missings?.Select(m => m.ToString()) });
        }
    }
}
