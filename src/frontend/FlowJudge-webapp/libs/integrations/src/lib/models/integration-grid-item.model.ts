import { DataGridRow } from '@flow-judge-webapp/ui';
import { IntegrationProvider, IntegrationStatus } from './integration-shared.model';
import { EnumValue } from '@flow-judge-webapp/common';

export interface IntegrationGridItem extends DataGridRow {
  name: string;
  provider: IntegrationProvider;
  status: EnumValue<IntegrationStatus>;
  createdAt: Date;
  creatorEmail: string;
}

