import { PageSize } from '@flow-judge-webapp/ui';

export class LoadRepositoriesGridItems {
  static readonly type = '[Repositories Grid] Load items';

  constructor(
    public readonly pageNumber: number,
    public readonly pageSize: PageSize,
  ) {}
}
