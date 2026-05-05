import { DataGridRow } from "@flow-judge-webapp/ui";
import { GetWorkspacesResponseItem, WorkspaceRole } from "../workspaces.model";

export interface WorkspaceGridItem extends DataGridRow {
  name: string;
  ownerEmail: string;
  role: WorkspaceRole;
  createdAt: Date;
}

export function MapDtoToModel(dto: GetWorkspacesResponseItem): WorkspaceGridItem {
  return {
    id: dto.workspaceId,
    name: dto.name,
    ownerEmail: dto.owner.emailAddress,
    role: dto.roleName,
    createdAt: dto.createdAt,
  } satisfies WorkspaceGridItem;
}
