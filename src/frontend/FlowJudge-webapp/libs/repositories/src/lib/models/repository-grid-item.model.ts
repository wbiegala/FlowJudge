import { EnumValue } from '@flow-judge-webapp/common';
import { IntegrationProvider, IntegrationStatus, RepositoryStatus } from './repository-shared.model';

export interface RepositoryGridItem {
  id: string;
  integration: RepositoryGridIntegrationItem;
  name: string;
  fullName: string | null;
  trackingEnabled: boolean;
  status: EnumValue<RepositoryStatus>;
}

export interface RepositoryGridIntegrationItem {
  id: string;
  name: string;
  provider: IntegrationProvider;
  status: EnumValue<IntegrationStatus>;
}
