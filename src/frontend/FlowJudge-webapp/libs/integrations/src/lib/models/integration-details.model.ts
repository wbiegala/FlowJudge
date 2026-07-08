import { EnumValue } from "@flow-judge-webapp/common";
import { IntegrationProvider, IntegrationStatus } from "./integration-shared.model";

export interface IntegrationDetails {
  id: string;
  workspaceId: string;
  name: string;
  provider: IntegrationProvider;
  status: EnumValue<IntegrationStatus>;
  createdAt: Date;
  creatorEmail: string;
  repositories: Array<IntegrationRepositoryDetails>;
}

export interface IntegrationRepositoryDetails {
  id: string;
  workspaceId: string;
  name: string;
  fullName?: string;
  trackingEnabled: boolean;
  status: EnumValue<'Active' | 'Deleted'>;
}
