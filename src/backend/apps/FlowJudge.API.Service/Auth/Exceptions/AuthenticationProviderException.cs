namespace FlowJudge.API.Service.Auth.Exceptions
{
    public sealed class AuthenticationProviderException : AuthenticationException
    {
        public AuthenticationProviderException(string message) : base(message) { }
        public AuthenticationProviderException(string message, Exception innerException) : base(message, innerException) { }
    }
}
