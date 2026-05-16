export class LoadIntegrationsGridItems {
  static readonly type = '[IntegrationsGrid] Loads data to grid';
  constructor(public pageNumber: number, public pageSize: number) {}
}
