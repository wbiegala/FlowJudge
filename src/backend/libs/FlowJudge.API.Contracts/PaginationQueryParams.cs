namespace FlowJudge.API.Contracts
{
    public sealed record PaginationQueryParams
    {
        public int PageNumber { get; init; } = 1;
        public int PageSize { get; init; } = 10;
    }
}
