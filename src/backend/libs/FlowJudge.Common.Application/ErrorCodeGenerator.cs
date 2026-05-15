namespace FlowJudge.Common.Application
{
    public static class ErrorCodeGenerator
    {
        public static string NotFound(string businessObjectName) =>
                $"{businessObjectName}.{ErrorCodes.NotFound}";

        public static string CreateFailed(string businessObjectName) =>
            $"{businessObjectName}.{ErrorCodes.CreateFailed}";

        public static string UpdateFailed(string businessObjectName) =>
            $"{businessObjectName}.{ErrorCodes.UpdateFailed}";

        public static string NotAcceptable(string businessObjectName) =>
            $"{businessObjectName}.{ErrorCodes.NotAcceptable}";

        public static string Forbidden(string businessObjectName) =>
            $"{businessObjectName}.{ErrorCodes.InsufficientPermissions}";
    }
}
