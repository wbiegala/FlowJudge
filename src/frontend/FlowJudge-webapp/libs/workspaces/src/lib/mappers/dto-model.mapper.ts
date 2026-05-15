import { match } from "ts-pattern";
import { WorkspaceDetails } from "../models/workspace-details.model";
import { GetWorkspaceResponse, GetWorkspacesResponseItem, WorkspaceUserData } from "../workspaces.model";
import { WorkspaceRole, WorkspaceStatus, WorkspaceUser } from "../models/workspace-shared.model";
import { EnumValue } from "@flow-judge-webapp/common";
import { WorkspaceGridItem } from "../models/workspace-grid-item.model";

export function mapToModel(dto: GetWorkspaceResponse): WorkspaceDetails {
  return {
    id: dto.id,
    name: dto.name,
    status: mapStatus(dto.status),
    createdAt: dto.createdAt,
    createdBy: mapUserData(dto.createdBy),
    members: dto.members.map(mdto => {
      return {
        user: mapUserData(mdto.member),
        role: mapRole(mdto.role),
        assingedAt: mdto.assingedAt,
        assignedBy: mdto.assignedBy ? mapUserData(mdto.assignedBy) : null
      };
    })
  } satisfies WorkspaceDetails;
}

export function MapGridDtoToModel(dto: GetWorkspacesResponseItem): WorkspaceGridItem {
  return {
    id: dto.workspaceId,
    name: dto.name,
    ownerEmail: dto.owner.emailAddress,
    role: mapRole(dto.roleName),
    createdAt: dto.createdAt,
  } satisfies WorkspaceGridItem;
}

function mapUserData(value: WorkspaceUserData): WorkspaceUser {
  return {
    id: value.userId,
    email: value.emailAddress,
    name: value.userName
  };
}

function mapStatus(value: WorkspaceStatus): EnumValue<WorkspaceStatus> {
  const translationKey = match(value)
    .with('Unactive', () => 'WORKSPACES.MODEL.STATUS.UNACTIVE')
    .with('Active', () => 'WORKSPACES.MODEL.STATUS.ACTIVE')
    .with('Archived', () => 'WORKSPACES.MODEL.STATUS.ARCHIVED')
    .exhaustive();

  return { value, translationKey };
}

function mapRole(value: WorkspaceRole): EnumValue<WorkspaceRole> {
  const translationKey = match(value)
    .with('Administrator', () => 'WORKSPACES.MODEL.ROLE.ADMINISTRATOR')
    .with('Member', () => 'WORKSPACES.MODEL.ROLE.MEMBER')
    .with('Owner', () => 'WORKSPACES.MODEL.ROLE.OWNER')
    .exhaustive();

  return { value, translationKey };
}
