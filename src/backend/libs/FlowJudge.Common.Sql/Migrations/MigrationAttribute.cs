namespace FlowJudge.Common.Sql.Migrations
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class MigrationAttribute : Attribute
    {
        public uint Number { get; }
        public string Description { get; }

        public MigrationAttribute(uint number, string description)
        {
            Number = number;
            Description = description;
        }
    }
}
