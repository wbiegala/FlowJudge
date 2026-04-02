using FlowJudge.Users.Domain.Model;

namespace FlowJudge.Users.Infrastructure.Repositories.DocumentVersions
{
    internal sealed record DocumentVersionDbModel
    {
        public Guid id { get; init; }
        public int number { get; init; }
        public string text_content { get; init; } = null!;
        public string html_content { get; init; } = null!;
        public DateTimeOffset creation_timestamp { get; init; }
        public bool is_acceptable { get; init; }
        public DocumentKindDbModel document_kind { get; init; }
    }

    internal static class DocumentVersionDbModelExtensions
    {
        public static DocumentVersion? ToDomainModel(this DocumentVersionDbModel dbModel)
        {
            return dbModel.document_kind switch
            {
                DocumentKindDbModel.PrivacyPolicy => PrivacyPolicyVersion.Load(
                    dbModel.id,
                    dbModel.number,
                    dbModel.text_content,
                    dbModel.html_content,
                    dbModel.creation_timestamp,
                    dbModel.is_acceptable),
                DocumentKindDbModel.TermsAndConditions => TermsAndConditionsVersion.Load(
                    dbModel.id,
                    dbModel.number,
                    dbModel.text_content,
                    dbModel.html_content,
                    dbModel.creation_timestamp,
                    dbModel.is_acceptable),
                _ => throw new InvalidOperationException($"Unknown DocumentKind: {dbModel.document_kind}")
            };
        }
    }
}
