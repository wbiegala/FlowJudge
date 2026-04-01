using FlowJudge.Common.Domain;

namespace FlowJudge.Shared.Domain.ValueObjects
{
    public abstract record GuidValueObject : ValueObject
    {
        public Guid Value { get; }

        protected GuidValueObject(Guid value)
        {
            if (value == Guid.Empty)
            {
                throw new ArgumentException("Value cannot be empty.", nameof(value));
            }

            Value = value;
        }

        public override string ToString() => Value.ToString();
    }
}
