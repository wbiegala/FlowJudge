import { WorkspaceMember, WorkspaceUser, WorkspaceStatus } from './workspace-shared.model';


export interface WorkspaceDetails {
  id: string | null;
  name: string;
  status: WorkspaceStatus;
  createdAt: Date;
  createdBy: WorkspaceUser;
  members: Array<WorkspaceMember>;
}
