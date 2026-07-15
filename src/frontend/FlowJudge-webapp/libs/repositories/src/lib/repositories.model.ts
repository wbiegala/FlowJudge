import { IntegrationProvider, IntegrationStatus, RepositoryStatus } from './models/repository-shared.model';

export interface GetRepositoriesResponseItem {
  id: string;
  integration: RepositoryIntegrationModel;
  workspaceId: string;
  name: string;
  fullName: string | null;
  trackingEnabled: boolean;
  status: RepositoryStatus;
}

export interface RepositoryIntegrationModel {
  integrationId: string;
  workspaceId: string;
  name: string;
  provider: IntegrationProvider,
  status: IntegrationStatus
}
