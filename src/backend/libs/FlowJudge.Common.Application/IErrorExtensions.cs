namespace FlowJudge.Common.Application
{
    public static class IErrorExtensions
    {
        public static bool IsNotFound(this IError error)
        {
            return ErrorCodeVarifier.NotFound(error.Code);
        }

        public static bool IsNotImplemented(this IError error)
        {
            return ErrorCodeVarifier.NotImplemented(error.Code);
        }

        public static bool IsInsufficientPermissions(this IError error)
        {
            return ErrorCodeVarifier.IsInsufficientPermissions(error.Code);
        }
    }
}
