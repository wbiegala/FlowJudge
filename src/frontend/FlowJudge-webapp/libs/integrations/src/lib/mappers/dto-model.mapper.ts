import { EnumValue } from "@flow-judge-webapp/common";
import { match } from "ts-pattern";
import { IntegrationStatus } from "../models/integration-shared.model";
import { GetIntegrationsResponseItem } from "../integrations.model";
import { IntegrationGridItem } from "../models/integration-grid-item.model";


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


function mapStatus(value: IntegrationStatus): EnumValue<IntegrationStatus> {
  const translationKey = match(value)
    .with('Inactive', () => 'INTEGRATIONS.MODEL.STATUS.INACTIVE')
    .with('Active', () => 'INTEGRATIONS.MODEL.STATUS.ACTIVE')
    .with('Deleted', () => 'INTEGRATIONS.MODEL.STATUS.DELETED')
    .exhaustive();

  return { value, translationKey };
}
