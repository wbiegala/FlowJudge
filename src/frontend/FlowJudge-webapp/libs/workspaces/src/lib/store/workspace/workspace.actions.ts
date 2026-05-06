export class InitializeNewWorkspace {
  static readonly type = '[Workspace] Initializes new workspace form';
}

export class InitializeEditWorkspace {
  static readonly type = '[Workspace] Initializes edit workspace form';
  constructor(public workspaceId: string) {}
}

export class SaveWorkspace {
  static readonly type = '[Workspace] Saves workspace';
}
