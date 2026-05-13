using FlowJudge.API.Contracts.User;
using FlowJudge.API.Service.Auth.Legal;
using FlowJudge.API.Service.ErrorHandling;
using FlowJudge.Common.Application.Mediator;
using FlowJudge.Common.Http.Extensions;
using FlowJudge.Users.Application.Abstractions.Commands;
using FlowJudge.Users.Application.Abstractions.Queries;
using FlowJudge.Users.Application.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace FlowJudge.API.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILegalLockManager _legalLockManager;

        public UserController(IMediator mediator, ILegalLockManager legalLockManager)
        {
            _mediator = mediator;
            _legalLockManager = legalLockManager;
        }

        [HttpPut("terms-and-conditions/{termsVersionId:guid}/accept")]
        [Authorize]
        [BypassLegalCheck]
        public async Task<IActionResult> AcceptTermsAndConditionsAsync(
            [FromRoute] Guid termsVersionId,
            CancellationToken cancellationToken = default)
        {
            var userContext = this.HttpContext.User.GetUserContext();
            var command = new AcceptTermsAndConditionsCommand(userContext.Id, termsVersionId);
            var result = await _mediator.SendCommandAsync(command, cancellationToken);

            if (!result.IsSuccess)
            {
                return result.Error!.ToResponse(HttpStatusCode.BadRequest);
            }

            await _legalLockManager.ReleaseLockAsync( userContext.Id, UserLegalRequirements.TermsAndConditionsActualVersionAccepted, cancellationToken);

            return Ok();
        }

        [HttpPut("privacy-policy/{privacyPolicyVersionId:guid}/accept")]
        [Authorize]
        [BypassLegalCheck]
        public async Task<IActionResult> AcceptPrivacyPolicyAsync(
            [FromRoute] Guid privacyPolicyVersionId,
            CancellationToken cancellationToken = default)
        {
            var userContext = this.HttpContext.User.GetUserContext();
            var command = new AcceptPrivacyPolicyCommand(userContext.Id, privacyPolicyVersionId);
            var result = await _mediator.SendCommandAsync(command, cancellationToken);

            if (!result.IsSuccess)
            {
                return result.Error!.ToResponse(HttpStatusCode.BadRequest);
            }

            await _legalLockManager.ReleaseLockAsync(userContext.Id, UserLegalRequirements.PrivacyPolicyActualVersionAccepted, cancellationToken);

            return Ok();
        }

        [HttpGet("legal-state")]
        [Authorize]
        [BypassLegalCheck]
        public async Task<IActionResult> GetUserLegalStateAsync(CancellationToken cancellationToken = default)
        {
            var userContext = this.HttpContext.User.GetUserContext();
            var query = new GetUserLegalStateQuery(userContext.Id);
            var result = await _mediator.SendQueryAsync<GetUserLegalStateQuery, GetUserLegalStateQueryResult>(query, cancellationToken);

            if (!result.IsSuccess)
            {

                return result.Error!.ToResponse(HttpStatusCode.BadRequest);
            }

            if (result.Data!.IsValid == false)
            {
                await _legalLockManager.AddLegalLockAsync(userContext.Id, result.Data.Missings!, cancellationToken);
            }

            return Ok(new GetUserLegalStateResponse { IsLegal = result.Data!.IsValid, Missings = result.Data?.Missings?.Select(m => m.ToString()) });
        }

        [HttpGet("ass")]
        [Authorize]
        public async Task<IActionResult> GetUserAssAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                return ex.ToProblemDetails(500, string.Empty).ToResponse();
            }
        }
    }
}
