export class CleanWorkspacesGrid {
  static readonly type = '[WorkspacesGrid] Cleans workspaces grid';
}

export class LoadWorkspacesGridItems {
  static readonly type = '[WorkspacesGrid] Loads data to grid';
  constructor(public pageNumber: number, public pageSize: number) {}
}
