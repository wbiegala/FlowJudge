using System.Net.Mail;

namespace FlowJudge.Shared.Domain.ValueObjects
{
    public sealed record EmailAddress : StringValueObject
    {
        private EmailAddress(string value) : base(value)
        {
        }

        public static EmailAddress Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Email address cannot be empty.", nameof(value));
            }

            var normalizedValue = value.Trim().ToLowerInvariant();

            if (!IsValid(normalizedValue))
            {
                throw new ArgumentException("Email address has invalid format.", nameof(value));
            }

            return new EmailAddress(normalizedValue);
        }

        public static bool TryCreate(string? value, out EmailAddress? emailAddress)
        {
            emailAddress = null;

            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            var normalizedValue = value.Trim().ToLowerInvariant();

            if (!IsValid(normalizedValue))
            {
                return false;
            }

            emailAddress = new EmailAddress(normalizedValue);
            return true;
        }

        public override string ToString() => Value;

        public static implicit operator string(EmailAddress emailAddress)
            => emailAddress.Value;

        public static explicit operator EmailAddress(string value)
            => Create(value);

        private static bool IsValid(string value)
        {
            try
            {
                var address = new MailAddress(value);
                return address.Address == value;
            }
            catch
            {
                return false;
            }
        }
    }
}
