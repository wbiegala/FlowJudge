using FlowJudge.Shared.Domain.ValueObjects;

namespace FlowJudge.Users.Domain.Model
{
    public sealed record UserName : StringValueObject
    {
        public const int MaxLength = 200;

        private UserName(string value) : base(value)
        {
        }

        public static UserName Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("User name cannot be empty.", nameof(value));
            }

            var normalizedValue = value.Trim();

            if (normalizedValue.Length > MaxLength)
            {
                throw new ArgumentException($"User name cannot be longer than {MaxLength} characters.", nameof(value));
            }

            return new UserName(normalizedValue);
        }

        public static bool TryCreate(string? value, out UserName? userName)
        {
            userName = null;

            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            var normalizedValue = value.Trim();

            if (normalizedValue.Length > MaxLength)
            {
                return false;
            }

            userName = new UserName(normalizedValue);
            return true;
        }

        public static implicit operator string(UserName userName)
            => userName.Value;

        public static explicit operator UserName(string value)
            => Create(value);
    }
}
