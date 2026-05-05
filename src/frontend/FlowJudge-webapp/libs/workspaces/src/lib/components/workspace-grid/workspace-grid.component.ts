import { DataGridAction, DataGridColumn, DataGridComponent, EmptyGridBehavior } from '@flow-judge-webapp/ui';
import { ChangeDetectionStrategy, Component, computed, effect, inject } from '@angular/core';
import { Store } from '@ngxs/store';
import { WorkspacesGridState } from '../../store/workspaces-grid/workspaces-grid.state';
import { LoadWorkspacesGridItems } from '../../store/workspaces-grid/workspaces-grid.actions';
import { WorkspaceGridItem } from '../../models/workspace-grid-item.model';

@Component({
  selector: 'lib-workspace-grid',
  imports: [DataGridComponent],
  templateUrl: './workspace-grid.component.html',
  styleUrl: './workspace-grid.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class WorkspaceGridComponent {
  #store = inject(Store);
  pageNumber = this.#store.selectSignal(WorkspacesGridState.pageNumber);
  pageSize = this.#store.selectSignal(WorkspacesGridState.pageSize);
  totalCount = this.#store.selectSignal(WorkspacesGridState.totalCount);
  totalPages = computed(() => this.totalCount() / this.pageSize());
  items = this.#store.selectSignal(WorkspacesGridState.items);
  isLoading = this.#store.selectSignal(WorkspacesGridState.isLoading);

  readonly #loadInitialData = effect(() => {
    this.#store.dispatch(new LoadWorkspacesGridItems(1, 25));
  });

  readonly gridColumns: Array<DataGridColumn<WorkspaceGridItem>> = [
    {
      id: 'name',
      name: 'name',
      headerTranslationKey: 'WORKSPACES.GRID.COLUMNS.NAME.HEADER',
      cell: row => row.name,
      isVisible: true,
      isSortable: false,
    },
    {
      id: 'ownerEmail',
      name: 'ownerEmail',
      headerTranslationKey: 'WORKSPACES.GRID.COLUMNS.OWNER.HEADER',
      cell: row => row.ownerEmail,
      isVisible: true,
      isSortable: false,
    },
    {
      id: 'role',
      name: 'role',
      headerTranslationKey: 'WORKSPACES.GRID.COLUMNS.ROLE.HEADER',
      cell: row => row.role,
      isVisible: true,
      isSortable: false,
    },
    {
      id: 'createdAt',
      name: 'createdAt',
      headerTranslationKey: 'WORKSPACES.GRID.COLUMNS.CREATED_AT.HEADER',
      cell: row => row.createdAt,
      isVisible: true,
      isSortable: false,
    }
  ];

  readonly gridActions: Array<DataGridAction> = [
    {
      name: 'add',
      nameTranslationKey: 'WORKSPACES.GRID.ACTIONS.ADD',
      icon: 'add',
      canExecute: () => true,
    }
  ];

  readonly emptyGridBehavior: EmptyGridBehavior = {
    messageTranslationKey: 'WORKSPACES.GRID.ON_EMPTY',
    actionName: 'add',
  };
}
