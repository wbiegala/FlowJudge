using FlowJudge.Users.Application.Abstractions.Ports;
using FlowJudge.Users.Domain.Model;

namespace FlowJudge.Users.Application.Extensions
{
    public static class IDocumentVersionRepositoryExtensions
    {
        public static async Task<TermsAndConditionsVersion?> GetTermsAndConditionsByIdAsync(
            this IDocumentVersionRepository repository,
            Guid versionId,
            CancellationToken cancellationToken = default)
        {
            var documentVersion = await repository.GetDocumentVersionByIdAsync(versionId, cancellationToken);
            if (documentVersion is null)
                return null;

            var terms = documentVersion as TermsAndConditionsVersion;

            return terms;
        }

        public static async Task<PrivacyPolicyVersion?> GetPrivacyPolicyByIdAsync(
            this IDocumentVersionRepository repository,
            Guid versionId,
            CancellationToken cancellationToken = default)
        {
            var documentVersion = await repository.GetDocumentVersionByIdAsync(versionId, cancellationToken);
            if (documentVersion is null)
                return null;

            var privacyPolicy = documentVersion as PrivacyPolicyVersion;

            return privacyPolicy;
        }

        public static async Task<TermsAndConditionsVersion?> GetActualTermsAndConditionsVersionAsync(
            this IDocumentVersionRepository repository,
            CancellationToken cancellationToken = default)
        {
            return await repository.GetActualDocumentVersionAsync<TermsAndConditionsVersion>(cancellationToken);
        }

        public static async Task<PrivacyPolicyVersion?> GetActualPrivacyPolicyVersionAsync(
            this IDocumentVersionRepository repository,
            CancellationToken cancellationToken = default)
        {
            return await repository.GetActualDocumentVersionAsync<PrivacyPolicyVersion>(cancellationToken);
        }
    }
}
