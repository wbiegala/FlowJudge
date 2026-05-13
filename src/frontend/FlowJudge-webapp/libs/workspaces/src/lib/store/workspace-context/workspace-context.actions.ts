export class SetWorkspaceContext {
  static readonly type = '[WorkspaceContext] Set workspace context';
  constructor(
    public workspaceId: string,
    public options: { showSuccessNotification?: boolean } = { showSuccessNotification: true },
  ) { }
}

export class CleanWorkspaceContext {
    static readonly type = '[WorkspaceContext] Clean workspace context';
}
