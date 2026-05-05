export class InitializeWorkspacesGrid {
  static readonly type = '[WorkspacesGrid] Initializes workspaces grid';
}

export class LoadWorkspacesGridItems {
  static readonly type = '[WorkspacesGrid] Loads data to grid';
  constructor(public pageNumber: number, public pageSize: number) {}
}
