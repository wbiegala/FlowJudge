import { Route } from '@angular/router';
import { provideStates } from '@ngxs/store';
import { IntegrationsGridState } from './store/integrations-grid/integrations-grid.state';
import { IntegrationDetailsState } from './store/integration-details/integration-details.state';
import { editIntegrationResolver } from './components/integration-details/integration-details.resolvers';

export const integrationsRoutes: Route[] = [
  {
    path: '',
    providers: [provideStates([IntegrationsGridState, IntegrationDetailsState])],
    children: [
      {
        path: '',
        loadComponent: () => import('./components/integration-grid/integration-grid.component').then(m => m.IntegrationGridComponent),
      },
      {
        path: ':id',
        loadComponent: () => import('./components/integration-details/integration-details.component').then(m => m.IntegrationDetailsComponent),
        resolve: { _: editIntegrationResolver }
      },
      {
        path: 'setup',
        children: [
          {
            path: 'github/:installationStateId',
            loadComponent: () => import('./components/setup-wizards/github-setup-wizard/github-setup-wizard.component').then(m => m.GitHubSetupWizardComponent),
          }
        ]
      }
    ]
  }
]
