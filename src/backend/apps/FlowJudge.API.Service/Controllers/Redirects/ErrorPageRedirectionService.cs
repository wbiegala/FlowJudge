using FlowJudge.Common.Application;
using FlowJudge.Common.Http.Extensions;

namespace FlowJudge.API.Service.Controllers.Redirects
{
    public class ErrorPageRedirectionService
    {
        private readonly string _uiBaseUrl;

        internal ErrorPageRedirectionService(string uiBaseUrl)
        {
            _uiBaseUrl = uiBaseUrl;
        }

        public string GetErrorPageReditectUrl(IError error)
        {
            var queryParams = new ErrorPageQueryParams
            {
                ErrorCode = error.Code,
                Message = error.Message,
            };

            var errorPageBaseUri = UriBuilderExtensions.CombineUri(_uiBaseUrl, "error").ToString();
            var uriBuidler = new UriBuilder(errorPageBaseUri);
            uriBuidler.AddQueryParams(queryParams);

            return uriBuidler.ToString();
        }

        private sealed record ErrorPageQueryParams
        {
            public required string ErrorCode { get; init; }
            public required string Message { get; init; }
        }
    }
}
