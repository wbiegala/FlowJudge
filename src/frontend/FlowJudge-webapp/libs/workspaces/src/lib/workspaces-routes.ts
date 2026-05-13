import { editWorkspaceResolver, newWorkspaceResolver } from './components/workspace-details/workspace-details.resolvers';
import { Route } from '@angular/router';
import { provideStates } from '@ngxs/store';
import { WorkspacesGridState } from './store/workspaces-grid/workspaces-grid.state';
import { WorkspaceState } from './store/workspace/workspace.state';

export const workspacesRoutes: Route[] = [
  {
    path: '',
    providers: [provideStates([WorkspacesGridState, WorkspaceState])],
    children: [
      {
        path: '',
        loadComponent: () => import('./components/workspace-grid/workspace-grid.component').then(m => m.WorkspaceGridComponent)
      },
      {
        path: 'new',
        loadComponent: () => import('./components/workspace-details/workspace-details.component').then(m => m.WorkspaceDetailsComponent),
        resolve: { _: newWorkspaceResolver }
      },
      {
        path: ':id',
        loadComponent: () => import('./components/workspace-details/workspace-details.component').then(m => m.WorkspaceDetailsComponent),
        resolve: { _: editWorkspaceResolver }
      }
    ]
  },
];
