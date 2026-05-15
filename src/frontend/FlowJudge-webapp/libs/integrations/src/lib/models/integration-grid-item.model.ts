import { EnumValue } from './../../../../common/src/lib/localization/enum-value.model';
import { DataGridRow } from '@flow-judge-webapp/ui';
import { IntegrationProvider, IntegrationStatus } from './integration-shared.model';

export interface IntegrationGridItem extends DataGridRow {
  name: string;
  provider: EnumValue<IntegrationProvider>;
  status: EnumValue<IntegrationStatus>;
  createdAt: Date;
  creatorEmail: string;
}

