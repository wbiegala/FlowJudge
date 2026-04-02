using FlowJudge.Common.Domain;

namespace FlowJudge.Users.Domain.Model
{
    public abstract class DocumentVersion : AggregateRoot
    {
        public int Number { get; private set; }
        public string TextContent { get; private set; }
        public string HtmlContent { get; private set; }
        public DateTimeOffset CreationTimestamp { get; private set; }
        public bool IsAcceptable { get; private set; }

        protected DocumentVersion(
            int number,
            string textContent,
            string htmlContent,
            DateTimeOffset creationTimestamp,
            bool isAcceptable)
        {
            Number = number;
            TextContent = textContent;
            HtmlContent = htmlContent;
            CreationTimestamp = creationTimestamp;
            IsAcceptable = isAcceptable;
        }
    }
}
