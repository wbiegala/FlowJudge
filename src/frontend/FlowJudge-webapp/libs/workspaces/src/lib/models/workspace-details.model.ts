import { EnumValue } from '@flow-judge-webapp/common';
import { WorkspaceMember, WorkspaceStatus, WorkspaceUser } from './workspace-shared.model';


export interface WorkspaceDetails {
  id: string | null;
  name: string;
  status: EnumValue<WorkspaceStatus>;
  createdAt: Date;
  createdBy: WorkspaceUser;
  members: Array<WorkspaceMember>;
}

