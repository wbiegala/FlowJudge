using FlowJudge.Common.Domain;

namespace FlowJudge.Shared.Domain.ValueObjects
{
    public abstract record StringValueObject : ValueObject
    {
        public string Value { get; }

        protected StringValueObject(string value)
        {
            Value = value;
        }

        public override string ToString() => Value;
    }
}
