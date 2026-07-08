import { EnumValue } from "@flow-judge-webapp/common";
import { match } from "ts-pattern";
import { IntegrationStatus } from "../models/integration-shared.model";
import { GetIntegrationDetailsResponse, GetIntegrationsResponseItem, GetRepositoriesResponseItem } from "../integrations.model";
import { IntegrationGridItem } from "../models/integration-grid-item.model";
import { IntegrationDetails, IntegrationRepositoryDetails } from "../models/integration-details.model";


export function MapGridDtoToModel(dto: GetIntegrationsResponseItem): IntegrationGridItem {
  return {
    id: dto.id,
    name: dto.name,
    provider: dto.provider,
    status: mapStatus(dto.status),
    createdAt: dto.createdAt,
    creatorEmail: dto.createdBy.emailAddress,
  };
}

export function MapToModel(dto: GetIntegrationDetailsResponse): IntegrationDetails {
  return {
    id: dto.id,
    workspaceId: dto.workspaceId,
    name: dto.name,
    provider: dto.provider,
    status: mapStatus(dto.status),
    createdAt: dto.createdAt,
    creatorEmail: dto.createdBy.emailAddress,
    repositories: dto.repositories.map(repo => mapToRepositoryModel(repo)),
  }
}


function mapStatus(value: IntegrationStatus): EnumValue<IntegrationStatus> {
  const translationKey = match(value)
    .with('Inactive', () => 'INTEGRATIONS.MODEL.STATUS.INACTIVE')
    .with('Active', () => 'INTEGRATIONS.MODEL.STATUS.ACTIVE')
    .with('Deleted', () => 'INTEGRATIONS.MODEL.STATUS.DELETED')
    .exhaustive();

  return { value, translationKey };
}

function mapToRepositoryModel(dto: GetRepositoriesResponseItem): IntegrationRepositoryDetails {
  return {
    id: dto.id,
    workspaceId: dto.workspaceId,
    name: dto.name,
    fullName: dto.fullName,
    trackingEnabled: dto.trackingEnabled,
    status: mapRepositoryStatus(dto.status)
  };
}

function mapRepositoryStatus(value: 'Active' | 'Deleted'): EnumValue<'Active' | 'Deleted'> {
  const translationKey = match(value)
    .with('Active', () => 'INTEGRATIONS.MODEL.STATUS.ACTIVE')
    .with('Deleted', () => 'INTEGRATIONS.MODEL.STATUS.DELETED')
    .exhaustive();

  return { value, translationKey };
}
