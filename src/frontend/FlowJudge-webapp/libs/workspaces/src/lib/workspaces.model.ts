export interface WorkspaceUserData {
  userId: string;
  userName: string;
  emailAddress: string;
}

export interface WorkspaceMemberData {
  member: WorkspaceUserData;
  role: WorkspaceRole;
  assignedBy?: WorkspaceUserData;
  assingedAt: Date;
}

export type WorkspaceRole = 'Member' | 'Administrator' | 'Owner';
export type WorkspaceStatus = 'Unactive' | 'Active' | 'Archived';

export interface GetWorkspacesResponseItem {
  workspaceId: string;
  name: string;
  owner: WorkspaceUserData;
  roleName: WorkspaceRole;
  createdAt: Date;
}

export interface GetWorkspaceResponse {
  id: string;
  name: string;
  status: WorkspaceStatus;
  createdAt: Date;
  createdBy: WorkspaceUserData;
  members: Array<WorkspaceMemberData>;
}

export interface CreateWorkspaceRequest {
  name: string;
}
