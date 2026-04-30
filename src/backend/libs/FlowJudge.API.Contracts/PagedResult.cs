namespace FlowJudge.API.Contracts
{
    public sealed record PagedResult<TItem>
    {
        public required int PageNumber { get; init; }
        public required int PageSize { get; init; }
        public required int TotalCount { get; init; }
        public required IEnumerable<TItem> Items { get; init; }
    }
}
