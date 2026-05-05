import { WorkspaceGridComponent } from './components/workspace-grid/workspace-grid.component';
import { Route } from '@angular/router';

export const workspacesRoutes: Route[] = [
  {
    path: '',
    component: WorkspaceGridComponent
  }
];
