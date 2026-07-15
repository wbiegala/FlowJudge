import { Route } from '@angular/router';
import { provideStates } from '@ngxs/store';
import { RepositoriesGridState } from './store/repositories-grid/repositories-grid.state';

export const repositoriesRoutes: Route[] = [
  {
    path: '',
    providers: [provideStates([RepositoriesGridState])],
    loadComponent: () => import('./components/repository-grid/repository-grid.component').then(m => m.RepositoryGridComponent),
  },
];
