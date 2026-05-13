export class SetWorkspaceContext {
  static readonly type = '[WorkspaceContext] Set workspace context';
  constructor(public workspaceId: string) { }
}

export class CleanWorkspaceContext {
    static readonly type = '[WorkspaceContext] Clean workspace context';
}
