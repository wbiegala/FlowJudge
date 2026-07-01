using System.Security.Cryptography;
using System.Text;

namespace FlowJudge.GitHub.Webhooks.Security
{
    internal sealed class GitHubWebhookSignatureValidator : IGitHubWebhookSignatureValidator
    {
        private readonly string _webhookSecret;

        public GitHubWebhookSignatureValidator(string webhookSecret)
        {
            _webhookSecret = webhookSecret;
        }

        public bool IsSignatureValid(GitHubWebhookMetadata metadata)
        {
            var bodyBytes = Encoding.UTF8.GetBytes(metadata.SerializedContent);
            var secretBytes = Encoding.UTF8.GetBytes(_webhookSecret);

            byte[] hmacBytes;
            if (metadata.SignatureHash == SignatureHashType.SHA256)
            {
                var hmac = new HMACSHA256(secretBytes);
                hmacBytes = hmac.ComputeHash(bodyBytes);
            }
            else
            {
                var hmac = new HMACSHA1(secretBytes);
                hmacBytes = hmac.ComputeHash(bodyBytes);
            }

            var prefix = metadata.SignatureHash switch
            {
                SignatureHashType.SHA1 => "sha1=",
                SignatureHashType.SHA256 => "sha256=",
                _ => throw new ArgumentOutOfRangeException(nameof(metadata.SignatureHash), "Unknown signature hash algorithm.")
            };

            var expectedSignature = prefix + Convert.ToHexString(hmacBytes).ToLowerInvariant();

            return CryptographicOperations.FixedTimeEquals(
                Encoding.UTF8.GetBytes(expectedSignature),
                Encoding.UTF8.GetBytes(metadata.Signature));
        }
    }
}
