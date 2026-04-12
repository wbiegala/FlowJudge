namespace FlowJudge.API.Service.Auth.Legal
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public sealed class BypassLegalCheckAttribute : Attribute
    {
    }
}
