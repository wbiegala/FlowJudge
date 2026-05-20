import { Route } from '@angular/router';
import { provideStates } from '@ngxs/store';
import { IntegrationsGridState } from './store/integrations-grid/integrations-grid.state';

export const integrationsRoutes: Route[] = [
  {
    path: '',
    providers: [provideStates([IntegrationsGridState])],
    children: [
      {
        path: '',
        loadComponent: () => import('./components/integration-grid/integration-grid.component').then(m => m.IntegrationGridComponent),
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
