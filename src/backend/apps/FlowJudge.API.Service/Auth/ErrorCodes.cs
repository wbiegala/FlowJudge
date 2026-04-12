namespace FlowJudge.API.Service.Auth
{
    public class ErrorCodes
    {
        public const string AuthenticationStateExceptionErrorCode = "auth.state_error";
        public const string TokenReceiveExceptionErrorCode = "auth.token_receive_error";
        public const string MissingRefreshTokenErrorCode = "auth.missing_refresh_token";

        public const string AuthorizationErrorsKey = "auth_error";
        public const string InsufficientPermissionsErrorCode = "auth.insufficient_permissions";
        public const string InvalidLegalStateErrorCode = "auth.invalid_legal_state";
    }
}
