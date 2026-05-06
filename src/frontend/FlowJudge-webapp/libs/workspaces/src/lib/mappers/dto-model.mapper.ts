import { match } from "ts-pattern";
import { WorkspaceDetails } from "../models/workspace-details.model";
import { GetWorkspaceResponse, WorkspaceRole, WorkspaceStatus, WorkspaceUserData } from "../workspaces.model";
import { WorkspaceAccessRole, WorkspaceUser } from "../models/workspace-shared.model";

export function mapToModel(dto: GetWorkspaceResponse): WorkspaceDetails {
  return {
    id: dto.id,
    name: dto.name,
    status: {
      value: dto.status,
      valueTranslationKey: getStatusTranslationKey(dto.status)
    },
    createdAt: dto.createdAt,
    createdBy: mapUserData(dto.createdBy),
    members: dto.members.map(mdto => {
      return {
        user: mapUserData(mdto.member),
        role: mapRoleData(mdto.role),
        assingedAt: mdto.assingedAt,
        assignedBy: mdto.assignedBy ? mapUserData(mdto.assignedBy) : null
      };
    })
  } satisfies WorkspaceDetails;
}

function mapUserData(value: WorkspaceUserData): WorkspaceUser {
  return {
    id: value.userId,
    email: value.emailAddress,
    name: value.userName
  };
}

function mapRoleData(value: WorkspaceRole): WorkspaceAccessRole {
  return {
    name: value,
    nameTranslationKey: getRoleTranslationKey(value)
  };
}

function getStatusTranslationKey(value: WorkspaceStatus): string {
  return match(value)
    .with('Unactive', () => 'WORKSPACES.MODEL.STATUS.UNACTIVE')
    .with('Active', () => 'WORKSPACES.MODEL.STATUS.ACTIVE')
    .with('Archived', () => 'WORKSPACES.MODEL.STATUS.ARCHIVED')
    .exhaustive();
}

function getRoleTranslationKey(value: WorkspaceRole): string {
  return match(value)
    .with('Administrator', () => 'WORKSPACES.MODEL.ROLE.ADMINISTRATOR')
    .with('Member', () => 'WORKSPACES.MODEL.ROLE.MEMBER')
    .with('Owner', () => 'WORKSPACES.MODEL.ROLE.OWNER')
    .exhaustive();
}
