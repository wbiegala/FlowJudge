using System.Reflection;
using FlowJudge.Common.Messaging.Outbox;
using FlowJudge.Common.Messaging.Outbox.SubjectMapping;

namespace FlowJudge.Common.Messaging
{
    public sealed class OutboxConfigurationWizard
    {
        private int _maxRetryCount = 5;
        private int _processingBatchSize = 100;
        private int _processingIntervalInSeconds = 10;

        private readonly List<OutboxSubjectMapping> _subjectMappings = new();


        internal OutboxConfiguration GetConfiguration()
        {
            return new OutboxConfiguration
            {
                MaxRetryCount = _maxRetryCount,
                ProcessingBatchSize = _processingBatchSize,
                ProcessingIntervalInSeconds = _processingIntervalInSeconds,
                SubjectMapping = _subjectMappings.AsReadOnly()
            };
        }

        /// <summary>
        /// Adds subject mapping for all messages in the specified assembly that have the OutboxSubjectAttribute defined.
        /// </summary>
        public void AddSubjectMappingForMessagesFromAssembly(Assembly assembly)
        {
            if (assembly == null) throw new ArgumentNullException(nameof(assembly));

            var messageInterface = typeof(IMessage);

            var types = assembly.GetTypes();

            foreach (var type in types)
            {
                if (!type.IsClass || type.IsAbstract) continue;
                if (!messageInterface.IsAssignableFrom(type)) continue;

                var attr = type.GetCustomAttribute<OutboxSubjectAttribute>(false);
                if (attr == null) continue;

                var typeName = type.AssemblyQualifiedName
                    ?? throw new InvalidOperationException($"Type {type.Name} does not have a full name.");

                if (_subjectMappings.Any(m => m.TypeName == typeName)) continue;

                _subjectMappings.Add(new OutboxSubjectMapping
                {
                    TypeName = typeName,
                    Subject = attr.Subject
                });
            }
        }

        public void AddSubjectMappingForMessageType<TMessage>(string subject)
        {
            _subjectMappings.Add(new OutboxSubjectMapping
            {
                TypeName = typeof(TMessage).AssemblyQualifiedName
                    ?? throw new InvalidOperationException($"Type {typeof(TMessage).Name} does not have a full name."),
                Subject = subject
            });
        }

        public void AddSubjectMappingForMessageType(Type messageType, string subject)
        {
            _subjectMappings.Add(new OutboxSubjectMapping
            {
                TypeName = messageType.AssemblyQualifiedName
                    ?? throw new InvalidOperationException($"Type {messageType.Name} does not have a full name."),
                Subject = subject
            });
        }

        public void AddSubjectMappingForMessageTypeName(string typeName, string subject)
        {
            _subjectMappings.Add(new OutboxSubjectMapping
            {
                TypeName = typeName,
                Subject = subject
            });
        }
    }
}
