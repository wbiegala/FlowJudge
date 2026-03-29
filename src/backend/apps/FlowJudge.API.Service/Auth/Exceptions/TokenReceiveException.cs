namespace FlowJudge.API.Service.Auth.Exceptions
{
    public class TokenReceiveException : AuthenticationException
    {
        public string ErrorDescription { get; }

        public TokenReceiveException(string message, string errorDescription) : base(message)
        {
            ErrorDescription = errorDescription;
        }
    }
}
