import { EnumValue } from "@flow-judge-webapp/common";
import { match } from "ts-pattern";
import { IntegrationProvider, IntegrationStatus } from "../models/integration-shared.model";
import { GetIntegrationsResponseItem } from "../integrations.model";
import { IntegrationGridItem } from "../models/integration-grid-item.model";


export function MapGridDtoToModel(dto: GetIntegrationsResponseItem): IntegrationGridItem {
  return {
    id: dto.id,
    name: dto.name,
    provider: mapProvider(dto.provider),
    status: mapStatus(dto.status),
    createdAt: dto.createdAt,
    creatorEmail: dto.createdBy.emailAddress,
  };
}

function mapProvider(value: IntegrationProvider): EnumValue<IntegrationProvider> {
  const key = match(value)
    .with('GitHub', () => 'WORKSPACES.MODEL.STATUS.UNACTIVE')
    .exhaustive();

  return { value: value, translationKey: key };
}

function mapStatus(value: IntegrationStatus): EnumValue<IntegrationStatus> {
  const key = match(value)
    .with('Inactive', () => 'WORKSPACES.MODEL.ROLE.ADMINISTRATOR')
    .with('Active', () => 'WORKSPACES.MODEL.ROLE.MEMBER')
    .with('Deleted', () => 'WORKSPACES.MODEL.ROLE.OWNER')
    .exhaustive();

  return { value, translationKey: key };
}
