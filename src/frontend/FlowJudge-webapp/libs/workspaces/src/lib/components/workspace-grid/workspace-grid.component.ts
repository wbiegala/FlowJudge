import { Navigate } from '@ngxs/router-plugin';
import { DataGridAction, DataGridActionEvent, DataGridColumn, DataGridComponent, DataGridRowAction, DataGridRowActionEvent, EmptyGridBehavior, PaginationComponent, PaginationEvent, ViewHeaderComponent } from '@flow-judge-webapp/ui';
import { ChangeDetectionStrategy, Component, effect, inject } from '@angular/core';
import { Store } from '@ngxs/store';
import { WorkspacesGridState } from '../../store/workspaces-grid/workspaces-grid.state';
import { LoadWorkspacesGridItems } from '../../store/workspaces-grid/workspaces-grid.actions';
import { WorkspaceGridItem } from '../../models/workspace-grid-item.model';
import { TranslatePipe } from '@ngx-translate/core';
import { SetWorkspaceContext } from '../../store/workspace-context/workspace-context.actions';

@Component({
  selector: 'lib-workspace-grid',
  imports: [ DataGridComponent, PaginationComponent, ViewHeaderComponent, TranslatePipe ],
  templateUrl: './workspace-grid.component.html',
  styleUrl: './workspace-grid.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class WorkspaceGridComponent {
  readonly titleKey = 'WORKSPACES.GRID.TITLE';
  #store = inject(Store);
  pageNumber = this.#store.selectSignal(WorkspacesGridState.pageNumber);
  pageSize = this.#store.selectSignal(WorkspacesGridState.pageSize);
  totalCount = this.#store.selectSignal(WorkspacesGridState.totalCount);
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

  readonly rowActions: Array<DataGridRowAction<WorkspaceGridItem>> = [
    {
      name: 'preview',
      nameTranslationKey: 'UI.VIEW_MODE.PREVIEW',
      icon: 'preview',
      canExecute: item => item.role !== 'Owner'
    },
    {
      name: 'edit',
      nameTranslationKey: 'UI.VIEW_MODE.EDIT',
      icon: 'edit',
      canExecute: item => item.role === 'Owner'
    },
    {
      name: 'setContext',
      nameTranslationKey: 'WORKSPACES.GRID.ROW_ACTIONS.SET_CONTEXT',
      icon: 'play_arrow',
      canExecute: item => true
    }
  ];

  readonly emptyGridBehavior: EmptyGridBehavior = {
    messageTranslationKey: 'WORKSPACES.GRID.ON_EMPTY',
    actionName: 'add',
  };

  handlePaginationEvent(event: PaginationEvent) {
    console.log(event);
  }

  handleRowAction(event: DataGridRowActionEvent) {
    switch (event.name) {
      case 'edit': this.#editWorkspace(event.id); break;
      case 'setContext': this.#setWorkspaceContext(event.id); break;
    }
  }

  handleGridEvent(event: DataGridActionEvent) {
    switch (event.name) {
      case 'add': this.#addNewWorkspace(); break;
      default:
        break;
    }
  }

  #addNewWorkspace() {
    this.#store.dispatch(new Navigate(['workspaces', 'new']));
  }

  #editWorkspace(id: string) {
    this.#store.dispatch(new Navigate(['workspaces', id]));
  }

  #setWorkspaceContext(id: string) {
    this.#store.dispatch(new SetWorkspaceContext(id));
  }
}
