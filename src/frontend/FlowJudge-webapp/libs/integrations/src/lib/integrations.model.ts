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

export interface GetIntegrationDetailsResponse {
  id: string;
  workspaceId: string;
  name: string;
  provider: IntegrationProvider;
  status: IntegrationStatus;
  createdAt: Date;
  createdBy: UserData;
  repositories: Array<GetRepositoriesResponseItem>;
}

export interface GetRepositoriesResponseItem {
  id: string;
  workspaceId: string;
  name: string;
  fullName?: string;
  trackingEnabled: boolean;
  status: 'Active' | 'Deleted';
}
