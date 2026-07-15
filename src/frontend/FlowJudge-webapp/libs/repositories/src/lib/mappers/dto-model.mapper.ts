import { EnumValue } from '@flow-judge-webapp/common';
import { match } from "ts-pattern";
import { RepositoryGridItem } from "../models/repository-grid-item.model";
import { IntegrationStatus, RepositoryStatus } from "../models/repository-shared.model";
import { GetRepositoriesResponseItem } from "../repositories.model";


export function MapGridDtoToModel(dto: GetRepositoriesResponseItem): RepositoryGridItem {
  return {
    id: dto.id,
    integration: {
      id: dto.integration.integrationId,
      name: dto.integration.name,
      provider: dto.integration.provider,
      status: mapIntegrationStatus(dto.integration.status),
    },
    name: dto.name,
    fullName: dto.fullName,
    trackingEnabled: dto.trackingEnabled,
    status: mapStatus(dto.status),
  };
}

function mapIntegrationStatus(value: IntegrationStatus): EnumValue<IntegrationStatus> {
  const translationKey = match(value)
    .with('Inactive', () => 'REPOSITORIES.MODEL.INTEGRATION_STATUS.INACTIVE')
    .with('Active', () => 'REPOSITORIES.MODEL.INTEGRATION_STATUS.ACTIVE')
    .with('Deleted', () => 'REPOSITORIES.MODEL.INTEGRATION_STATUS.DELETED')
    .exhaustive();

  return { value, translationKey };
}

function mapStatus(value: RepositoryStatus): EnumValue<RepositoryStatus> {
  const translationKey = match(value)
    .with('Active', () => '')
    .with('Deleted', () => '')
    .exhaustive();

    return { value, translationKey };
}
