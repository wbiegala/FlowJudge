export interface PaginationQueryParams {
  pageNumber: number;
  pageSize: number;
}

export interface PagedResult<TModel> {
  pageNumber: number;
  pageSize: number;
  totalCount: number;
  items: Array<TModel>;
}
