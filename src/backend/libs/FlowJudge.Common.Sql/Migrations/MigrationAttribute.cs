namespace FlowJudge.Common.Sql.Migrations
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class MigrationAttribute : Attribute
    {
        public int Number { get; }
        public string Description { get; }

        public MigrationAttribute(int number, string description)
        {
            Number = number;
            Description = description;
        }
    }
}
