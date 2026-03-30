namespace FlowJudge.Common.Application
{
    public static class ApplicationResultFactory
    {
        public static IResult Success()
        {
            return new ApplicationResult
            {
                IsSuccess = true,
            };
        }

        public static IResult<TResult> Success<TResult>(TResult data)
        {
            return new ApplicationResult<TResult>
            {
                IsSuccess = true,
                Data = data,
            };
        }

        public static IResult Failure(IError error)
        {
            return new ApplicationResult
            {
                IsSuccess = false,
                Error = error
            };
        }

        public static IResult Failure(
            Exception exception,
            string code,
            IDictionary<string, object>? properties = null)
        {
            return new ApplicationResult
            {
                IsSuccess = false,
                Error = new ApplicationError
                {
                    Code = code,
                    Message = exception.Message,
                    Properties = properties,
                    Exception = exception
                }
            };
        }

        public static IResult Failure(
            string message,
            string code,
            IDictionary<string, object>? properties = null)
        {
            return new ApplicationResult
            {
                IsSuccess = false,
                Error = new ApplicationError
                {
                    Code = code,
                    Message = message,
                    Properties = properties
                }
            };
        }

        public static IResult<TResult> Failure<TResult>(IError error)
        {
            return new ApplicationResult<TResult>
            {
                IsSuccess = false,
                Error = error
            };
        }

        public static IResult<TResult> Failure<TResult>(
            Exception exception,
            string code,
            IDictionary<string, object>? properties = null)
        {
            return new ApplicationResult<TResult>
            {
                IsSuccess = false,
                Error = new ApplicationError
                {
                    Code = code,
                    Message = exception.Message,
                    Properties = properties,
                    Exception = exception
                }
            };
        }

        public static IResult<TResult> Failure<TResult>(
            string message,
            string code,
            IDictionary<string, object>? properties = null)
        {
            return new ApplicationResult<TResult>
            {
                IsSuccess = false,
                Error = new ApplicationError
                {
                    Code = code,
                    Message = message,
                    Properties = properties
                }
            };
        }
    }
}
