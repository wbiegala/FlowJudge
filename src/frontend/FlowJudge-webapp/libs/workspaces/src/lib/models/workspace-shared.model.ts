export interface WorkspaceStatus {
  value: string;
  valueTranslationKey: string;
}

export interface WorkspaceAccessRole {
  name: string;
  nameTranslationKey: string;
}

export interface WorkspaceUser {
  id: string;
  name: string;
  email: string;
}

export interface WorkspaceMember {
  user: WorkspaceUser;
  role: WorkspaceAccessRole;
  assignedBy: WorkspaceUser | null;
  assingedAt: Date;
}
