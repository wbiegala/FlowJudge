import { UpdateIntegrationRequest } from '../integrations.model';
import { IntegrationDetails } from './../models/integration-details.model';

export function MapToUpdateRequest(model: IntegrationDetails): UpdateIntegrationRequest {
  return {
    name: model.name,
    status: model.status.value,
    repositoriesTrackingSettings: model.repositories.map(repo => {
      return {
        repositoryId: repo.id,
        trackingEnabled: repo.trackingEnabled
      };
    })
  }
}
