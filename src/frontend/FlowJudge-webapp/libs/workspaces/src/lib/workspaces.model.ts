import { WorkspaceRole, WorkspaceStatus } from './models/workspace-shared.model';

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

export interface UpdateWorkspaceRequest {
  name: string;
}
