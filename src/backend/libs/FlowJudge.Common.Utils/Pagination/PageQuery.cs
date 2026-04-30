namespace FlowJudge.Common.Utils.Pagination
{
    public record PageQuery
    {
        public required int PageSize { get; init; }
        public required int PageNumber { get; init; }
    }
}
