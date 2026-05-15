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
      }
    ]
  }
]
