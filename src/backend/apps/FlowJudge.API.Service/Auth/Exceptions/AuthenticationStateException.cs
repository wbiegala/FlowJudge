namespace FlowJudge.API.Service.Auth.Exceptions
{
    public class AuthenticationStateException : AuthenticationException
    {
        public Guid StateId { get; }

        public AuthenticationStateException(Guid stateId, string message) : base($"stateId={stateId}: {message}")
        {
            StateId = stateId;
        }

        public AuthenticationStateException(Guid stateId, string message, Exception innerException) :
            base($"stateId={stateId}: {message}", innerException)
        {
            StateId = stateId;
        }
    }
}
