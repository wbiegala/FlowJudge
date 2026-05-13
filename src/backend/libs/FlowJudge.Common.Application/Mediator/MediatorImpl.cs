using Microsoft.Extensions.DependencyInjection;
using System.Collections;

namespace FlowJudge.Common.Application.Mediator
{
    internal sealed class MediatorImpl : IMediator
    {
        private readonly IServiceProvider _serviceProvider;

        public MediatorImpl(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Task<IResult> SendCommandAsync<TCommand>(
            TCommand command,
            CancellationToken cancellationToken = default)
                where TCommand : ICommand
        {
            var handler = _serviceProvider.GetService<ICommandHandler<TCommand>>();

            if (handler is null)
            {
                var error = new MissingHandlerErrorResult { Error = new MissingHandlerError() };

                return Task.FromResult<IResult>(error);
            }

            return handler.ExecuteAsync(command, cancellationToken);
        }

        public Task<IResult<TResult>> SendCommandAsync<TCommand, TResult>(
            TCommand command,
            CancellationToken cancellationToken = default)
                where TCommand : ICommand<TResult>
        {
            var handler = _serviceProvider.GetService<ICommandHandler<TCommand, TResult>>();

            if (handler is null)
            {
                var error = new MissingHandlerErrorResult<TResult> { Error = new MissingHandlerError() };

                return Task.FromResult<IResult<TResult>>(error);
            }

            return handler.ExecuteAsync(command, cancellationToken);
        }

        public Task<IResult<TResult>> SendQueryAsync<TQuery, TResult>(
            TQuery query,
            CancellationToken cancellationToken = default)
                where TQuery : IQuery<TResult>
        {
            var handler = _serviceProvider.GetService<IQueryHandler<TQuery, TResult>>();

            if (handler is null)
            {
                var error = new MissingHandlerErrorResult<TResult> { Error = new MissingHandlerError() };

                return Task.FromResult<IResult<TResult>>(error);
            }

            return handler.HandleAsync(query, cancellationToken);
        }


        private sealed record MissingHandlerErrorResult : IResult
        {
            public bool IsSuccess => false;

            public IError? Error { get; init; }
        }

        private sealed record MissingHandlerErrorResult<TResult> : IResult<TResult>
        {
            public bool IsSuccess => false;

            public IError? Error { get; init; }

            public TResult? Data => default;
        }

        private sealed record MissingHandlerError : IError
        {
            public string Code => ErrorCodes.NotImplemented;
            public string Message => "Missing handler.";
            public IDictionary<string, object>? Properties => null;
            public Exception? Exception => null;
        }
    }
}
