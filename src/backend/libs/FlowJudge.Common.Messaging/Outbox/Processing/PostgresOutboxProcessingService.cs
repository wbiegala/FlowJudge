using Dapper;
using FlowJudge.Common.Messaging.Outbox.Model;
using FlowJudge.Common.Messaging.Publishing;
using FlowJudge.Common.Sql.UnitOfWork;
using FlowJudge.Common.Utils.Time;

namespace FlowJudge.Common.Messaging.Outbox.Processing
{
    internal sealed class PostgresOutboxProcessingService : DapperRepository, IOutboxProcessingService
    {
        private readonly IOutboxMessagePublisher _publisher;
        private readonly ITimeService _timeService;
        private readonly OutboxConfiguration _configuration;
        private readonly ISqlSession _session;

        public PostgresOutboxProcessingService(
            IOutboxMessagePublisher publisher,
            ITimeService timeService,
            OutboxConfiguration configuration,
            ISqlSession sqlSession) : base(sqlSession)
        {
            _publisher = publisher;
            _timeService = timeService;
            _configuration = configuration;
            _session = sqlSession;
        }

        public async Task ProcessBatchAsync(CancellationToken cancellationToken = default)
        {
            var batch = await GetUnsentMessagesAsync(cancellationToken);

            foreach (var message in batch)
            {
                await ProcessMessageAsync(message, cancellationToken);
            }
        }

        private async Task ProcessMessageAsync(OutboxMessage message, CancellationToken cancellationToken)
        {
            var subjectMap = _configuration.SubjectMapping.FirstOrDefault(sm => sm.TypeName == message.Type);

            if (subjectMap is null)
            {
                await NotifyFailAsync(message, true, "No subject mapped with message type", cancellationToken);
                return;
            }

            try
            {
                await _publisher.PublishOutboxMessageAsync(
                    message.SystemId, message.Type, message.Payload, subjectMap.Subject, cancellationToken);
                await NotifySuccessAsync(message, cancellationToken);
            }
            catch (PublishMessageException pex)
            {
                await NotifyFailAsync(message, false, pex.Reason ?? pex.Message, cancellationToken);
            }
            catch (Exception ex)
            {
                await NotifyFailAsync(message, false, ex.Message, cancellationToken);
            }
        }

        private async Task NotifySuccessAsync(OutboxMessage message, CancellationToken cancellationToken)
        {
            const string setTimestampSql = $@"
UPDATE {OutboxConfiguration.SchemaName}.{OutboxConfiguration.OutboxMessageTableName}
SET processing_timestamp = @Timestamp
WHERE id = @MessageId;";

            const string addSuccessLogSql = $@"
INSERT INTO {OutboxConfiguration.SchemaName}.{OutboxConfiguration.OutboxMessageLogTableName}
(
     id
    ,outbox_message_id
    ,occured_timestamp
    ,result
) VALUES (
     @{nameof(OutboxMessageLog.Id)}
    ,@{nameof(OutboxMessageLog.OutboxMessageId)}
    ,@{nameof(OutboxMessageLog.Timestamp)}
    ,@{nameof(OutboxMessageLog.ProcessingResult)}
);";

            var timestamp = _timeService.UtcNow;
            await EnsureConnectionOpenAsync(cancellationToken);
            await _session.BeginTransactionAsync(cancellationToken);

            try
            {
                var setTimestampCommand = Command(setTimestampSql, new { MessageId = message.Id, Timestamp = timestamp }, cancellationToken);
                await Connection.ExecuteAsync(setTimestampCommand);

                var log = new OutboxMessageLog
                {
                    Id = Guid.NewGuid(),
                    OutboxMessageId = message.Id,
                    Timestamp = timestamp,
                    ProcessingResult = OutboxMessageProcessingResult.Sent,
                };
                var addSuccessLogCommand = Command(addSuccessLogSql, log, cancellationToken);
                await Connection.ExecuteAsync(addSuccessLogCommand);

                await _session.CommitAsync(cancellationToken);
            }
            catch (Exception)
            {
                await _session.RollbackAsync(cancellationToken);
                throw;
            }
        }

        private async Task NotifyFailAsync(OutboxMessage message, bool skipped, string reason, CancellationToken cancellationToken)
        {
            const string addFailureLogSql = $@"
INSERT INTO {OutboxConfiguration.SchemaName}.{OutboxConfiguration.OutboxMessageLogTableName}
(
     id
    ,outbox_message_id
    ,occured_timestamp
    ,result
    ,error_details
) VALUES (
     @{nameof(OutboxMessageLog.Id)}
    ,@{nameof(OutboxMessageLog.OutboxMessageId)}
    ,@{nameof(OutboxMessageLog.Timestamp)}
    ,@{nameof(OutboxMessageLog.ProcessingResult)}
    ,@{nameof(OutboxMessageLog.ErrorDetails)}
);";

            var timestamp = _timeService.UtcNow;
            await EnsureConnectionOpenAsync(cancellationToken);

            var log = new OutboxMessageLog
            {
                Id = Guid.NewGuid(),
                OutboxMessageId = message.Id,
                Timestamp = timestamp,
                ProcessingResult = skipped ? OutboxMessageProcessingResult.Skipped : OutboxMessageProcessingResult.Error,
                ErrorDetails = reason
            };

            var addSuccessLogCommand = Command(addFailureLogSql, log, cancellationToken);
            await Connection.ExecuteAsync(addSuccessLogCommand);
        }


        private async Task<IEnumerable<OutboxMessage>> GetUnsentMessagesAsync(CancellationToken cancellationToken)
        {
            const string getMessagesQuery = @$"
SELECT 
     id                     as {nameof(OutboxMessage.Id)}
    ,type                   as {nameof(OutboxMessage.Type)}
    ,system_id              as {nameof(OutboxMessage.SystemId)}
    ,payload                as {nameof(OutboxMessage.Payload)}
    ,publication_timestamp  as {nameof(OutboxMessage.PublicationTimestamp)}
    ,processing_timestamp   as {nameof(OutboxMessage.ProcessingTimestamp)}
FROM {OutboxConfiguration.SchemaName}.{OutboxConfiguration.OutboxMessageTableName} om
WHERE processing_timestamp IS NULL AND (
    SELECT count(*)
    FROM {OutboxConfiguration.SchemaName}.{OutboxConfiguration.OutboxMessageLogTableName} oml
    WHERE outbox_message_id = om.id AND result <> 200) < @TryoutsCount
ORDER BY publication_timestamp ASC
LIMIT @BatchSize";

            await EnsureConnectionOpenAsync(cancellationToken);
            var command = Command(getMessagesQuery,
                new { BatchSize = _configuration.ProcessingBatchSize, TryoutsCount = _configuration.MaxRetryCount }, cancellationToken);

            return await Connection.QueryAsync<OutboxMessage>(command);
        }

    }
}
