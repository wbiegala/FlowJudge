namespace FlowJudge.API.Service.Auth.Common
{
    internal static class Constants
    {
        internal static class ContentTypes
        {
            public const string HEADER = "Content-Type";
            public const string ApplicationJson = "application/json";
            public const string ApplicationFormUrlEncoded = "application/x-www-form-urlencoded";
        }

        internal static class GrantTypes
        {
            public const string GRANT_TYPE = "grant_type";
            public const string AuthorizationCode = "authorization_code";
            public const string RefreshToken = "refresh_token";
        }

        internal static class Authorization
        {
            public const string Code = "code";
            public const string ClientId = "client_id";
            public const string ClientSecret = "client_secret";
            public const string RedirectUri = "redirect_uri";
            public const string Refresh = "refresh_token";
        }
    }
}
