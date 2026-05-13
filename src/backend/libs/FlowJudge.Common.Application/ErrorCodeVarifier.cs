namespace FlowJudge.Common.Application
{
    public static class ErrorCodeVarifier
    {
        public static bool NotFound(string errorCode)
        {
            return errorCode.EndsWith(ErrorCodes.NotFound);
        }

        public static bool NotImplemented(string errorCode)
        {
            return errorCode.EndsWith(ErrorCodes.NotImplemented);
        }

        public static bool IsInsufficientPermissions(string errorCode)
        {
            return errorCode.EndsWith(ErrorCodes.InsufficientPermissions);
        }
    }
}
