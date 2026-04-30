using FlowJudge.API.Contracts;
using FlowJudge.Common.Utils.Pagination;

namespace FlowJudge.API.Service.Controllers.Mappers
{
    internal static class PaginationMapper
    {
        public static PageQuery ToModel(this PaginationQueryParams queryParams) =>
            new PageQuery { PageNumber = queryParams.PageNumber, PageSize = queryParams.PageSize };

        public static PagedResult<TContract> ToPagedResult<TItem, TContract>(
            this PagedList<TItem> pagedList,
            Func<TItem, TContract> mapToContract)
        {
            return new PagedResult<TContract>
            {
                PageNumber = pagedList.PageNumber,
                PageSize = pagedList.PageSize,
                TotalCount = pagedList.TotalCount,
                Items = pagedList.Select(item => mapToContract(item)).ToArray()
            };
        }
    }
}
