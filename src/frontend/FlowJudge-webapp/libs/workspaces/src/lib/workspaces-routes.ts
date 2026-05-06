import { WorkspaceDetailsComponent } from './components/workspace-details/workspace-details.component';
import { editWorkspaceResolver, newWorkspaceResolver } from './components/workspace-details/workspace-details.resolvers';
import { WorkspaceGridComponent } from './components/workspace-grid/workspace-grid.component';
import { Route } from '@angular/router';

export const workspacesRoutes: Route[] = [
  {
    path: '',
    component: WorkspaceGridComponent
  },
  {
    path: 'new',
    component: WorkspaceDetailsComponent,
    resolve: { _: newWorkspaceResolver }
  },
  {
    path: ':id',
    component: WorkspaceDetailsComponent,
    resolve: { _: editWorkspaceResolver }
  }
];
