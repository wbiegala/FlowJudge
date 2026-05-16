import { ChangeDetectionStrategy, Component, effect, inject } from '@angular/core';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { Store } from '@ngxs/store';
import { DataGridAction, DataGridActionEvent, DataGridColumn, DataGridComponent, DataGridRowActionEvent, EmptyGridBehavior, PaginationComponent, PaginationEvent, ViewHeaderComponent } from '@flow-judge-webapp/ui';
import { IntegrationsGridState } from '../../store/integrations-grid/integrations-grid.state';
import { LoadIntegrationsGridItems } from '../../store/integrations-grid/integrations-grid.actions';
import { IntegrationGridItem } from '../../models/integration-grid-item.model';
import { formatDateTime } from '@flow-judge-webapp/common';

@Component({
  selector: 'lib-integration-grid',
  imports: [ DataGridComponent, PaginationComponent, ViewHeaderComponent, TranslatePipe ],
  templateUrl: './integration-grid.component.html',
  styleUrl: './integration-grid.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class IntegrationGridComponent {
  readonly title = 'INTEGRATIONS.GRID.TITLE';
  #store = inject(Store);
  #translateService = inject(TranslateService);
  pageNumber = this.#store.selectSignal(IntegrationsGridState.pageNumber);
  pageSize = this.#store.selectSignal(IntegrationsGridState.pageSize);
  totalCount = this.#store.selectSignal(IntegrationsGridState.totalCount);
  items = this.#store.selectSignal(IntegrationsGridState.items);
  isLoading = this.#store.selectSignal(IntegrationsGridState.isLoading);

  readonly #loadInitialData = effect(() => {
    this.#store.dispatch(new LoadIntegrationsGridItems(1, 25));
  });

  readonly gridColumns: Array<DataGridColumn<IntegrationGridItem>> = [
    {
      id: 'name',
      name: 'name',
      headerTranslationKey: 'INTEGRATIONS.GRID.COLUMNS.NAME.HEADER',
      cell: row => row.name,
      isVisible: true,
      isSortable: false,
    },
    {
      id: 'ownerEmail',
      name: 'ownerEmail',
      headerTranslationKey: 'INTEGRATIONS.GRID.COLUMNS.PROVIDER.HEADER',
      cell: row => row.provider,
      isVisible: true,
      isSortable: false,
    },
    {
      id: 'role',
      name: 'role',
      headerTranslationKey: 'INTEGRATIONS.GRID.COLUMNS.STATUS.HEADER',
      cell: row => this.#translateService.instant(row.status.translationKey),
      isVisible: true,
      isSortable: false,
    },
    {
      id: 'createdAt',
      name: 'createdAt',
      headerTranslationKey: 'INTEGRATIONS.GRID.COLUMNS.CREATED.HEADER',
      cell: row => this.#formatCreatedCell(row.creatorEmail, row.createdAt),
      isVisible: true,
      isSortable: false,
    }
  ];

  readonly gridActions: Array<DataGridAction> = [
    {
      name: 'addGitHub',
      nameTranslationKey: 'INTEGRATIONS.GRID.ACTIONS.ADD_GITHUB',
      icon: 'add',
      canExecute: () => true,
    }
  ];

  readonly emptyGridBehavior: EmptyGridBehavior = {
    messageTranslationKey: 'INTEGRATIONS.GRID.ON_EMPTY',
    actionName: 'addGitHub',
  };

  handlePaginationEvent(event: PaginationEvent) {
    console.log(event);
  }

  handleRowAction(event: DataGridRowActionEvent) {
    console.log(event);
  }

  handleGridEvent(event: DataGridActionEvent) {
    switch (event.name) {
      case 'addGitHub': this.#addGitHubIntegration(); break;
      default:
        break;
    }
  }

  #addGitHubIntegration() {
    console.log('add github');
  }

  #formatCreatedCell(creator: string, createdAt: Date) {
    return `${creator}\n${formatDateTime(createdAt)}`;
  }
}
