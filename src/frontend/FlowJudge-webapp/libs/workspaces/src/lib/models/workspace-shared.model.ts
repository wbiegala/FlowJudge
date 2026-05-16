import { EnumValue } from "@flow-judge-webapp/common";

export type WorkspaceRole = 'Member' | 'Administrator' | 'Owner';
export type WorkspaceStatus = 'Unactive' | 'Active' | 'Archived';

export interface WorkspaceUser {
  id: string;
  name: string;
  email: string;
}

export interface WorkspaceMember {
  user: WorkspaceUser;
  role: EnumValue<WorkspaceRole>;
  assignedBy: WorkspaceUser | null;
  assingedAt: Date;
}
