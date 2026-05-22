import { IntegrationProvider, IntegrationStatus } from './models/integration-shared.model';

export interface UserData {
  userId: string;
  userName: string;
  emailAddress: string;
}

export interface GetIntegrationsResponseItem {
  id: string;
  name: string;
  provider: IntegrationProvider;
  status: IntegrationStatus;
  createdAt: Date;
  createdBy: UserData;
}
