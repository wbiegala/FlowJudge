using Dapper;
using FlowJudge.Common.Messaging.Outbox.Model;
using FlowJudge.Common.Sql.UnitOfWork;
using FlowJudge.Common.Utils.Time;

namespace FlowJudge.Common.Messaging.Outbox.Impl
{
    internal sealed class PostgresOutbox
        : DapperRepository, IOutbox
    {
        private readonly ITimeService _timeService;

        public PostgresOutbox(ISqlSession sqlSession, ITimeService timeService) : base(sqlSession)
        {
            _timeService = timeService;
        }

        public async Task PublishAsync(IMessage message, CancellationToken cancellationToken = default)
        {
            var outboxMessage = new OutboxMessage
            {
                Id = Guid.NewGuid(),
                Type = SerializationHelper.GetType(message),
                SystemId = message.MessageId,
                Payload = SerializationHelper.Serialize(message),
                PublicationTimestamp = _timeService.UtcNow,
            };

            await EnsureConnectionOpenAsync(cancellationToken);

            await Connection.ExecuteAsync(Command(InsertSql, outboxMessage, cancellationToken));
        }

        private const string InsertSql = $@"
INSERT INTO {OutboxConfiguration.SchemaName}.{OutboxConfiguration.OutboxMessageTableName} (
     id
    ,type
    ,system_id
    ,payload
    ,publication_timestamp
)
VALUES (
     @{nameof(OutboxMessage.Id)}
    ,@{nameof(OutboxMessage.Type)}
    ,@{nameof(OutboxMessage.SystemId)}
    ,@{nameof(OutboxMessage.Payload)}
    ,@{nameof(OutboxMessage.PublicationTimestamp)}
);";
    }
}
