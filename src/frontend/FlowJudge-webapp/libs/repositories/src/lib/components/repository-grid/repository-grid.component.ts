import { ChangeDetectionStrategy, Component, effect, inject } from '@angular/core';
import { Store } from '@ngxs/store';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import {
  DataGridColumn,
  DataGridComponent,
  EmptyGridBehavior,
  IconCell,
  PaginationComponent,
  PaginationEvent,
  TwoLinesCell,
  ViewHeaderComponent,
} from '@flow-judge-webapp/ui';
import { RepositoryGridItem } from '../../models/repository-grid-item.model';
import { LoadRepositoriesGridItems } from '../../store/repositories-grid/repositories-grid.actions';
import { RepositoriesGridState } from '../../store/repositories-grid/repositories-grid.state';

@Component({
  selector: 'lib-repository-grid',
  imports: [DataGridComponent, PaginationComponent, ViewHeaderComponent, TranslatePipe],
  templateUrl: './repository-grid.component.html',
  styleUrl: './repository-grid.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class RepositoryGridComponent {
  readonly title = 'REPOSITORIES.GRID.TITLE';
  #store = inject(Store);
  #translateService = inject(TranslateService);

  pageSize = this.#store.selectSignal(RepositoriesGridState.pageSize);
  totalCount = this.#store.selectSignal(RepositoriesGridState.totalCount);
  items = this.#store.selectSignal(RepositoriesGridState.items);
  isLoading = this.#store.selectSignal(RepositoriesGridState.isLoading);

  readonly loadInitialData = effect(() => {
    this.#store.dispatch(new LoadRepositoriesGridItems(1, 25));
  });

  readonly gridColumns: Array<DataGridColumn<RepositoryGridItem>> = [
    {
      id: 'name',
      name: 'name',
      headerTranslationKey: 'REPOSITORIES.GRID.COLUMNS.NAME',
      cell: row => ({ firstLine: row.name, secondLine: row.fullName ?? '' } satisfies TwoLinesCell),
      isVisible: true,
      isSortable: false,
    },
    {
      id: 'integration',
      name: 'integration',
      headerTranslationKey: 'REPOSITORIES.GRID.COLUMNS.INTEGRATION',
      cell: row => ({
        icon: 'github',
        isSvgIcon: true,
        text: row.integration.name,
        valueTranslationKey: row.integration.provider,
      } satisfies IconCell),
      isVisible: true,
      isSortable: false,
    },
    {
      id: 'trackingEnabled',
      name: 'trackingEnabled',
      headerTranslationKey: 'REPOSITORIES.GRID.COLUMNS.TRACKING_ENABLED',
      cell: row => ({
        icon: row.trackingEnabled ? 'check' : 'block',
        valueTranslationKey: row.trackingEnabled
          ? 'REPOSITORIES.GRID.TRACKING.ENABLED'
          : 'REPOSITORIES.GRID.TRACKING.DISABLED',
      } satisfies IconCell),
      isVisible: true,
      isSortable: false,
      textAlign: 'center',
    },
    {
      id: 'status',
      name: 'status',
      headerTranslationKey: 'REPOSITORIES.GRID.COLUMNS.STATUS',
      cell: row => this.#translateService.instant(row.status.translationKey),
      isVisible: true,
      isSortable: false,
    },
  ];

  readonly emptyGridBehavior: EmptyGridBehavior = {
    messageTranslationKey: 'REPOSITORIES.GRID.ON_EMPTY',
    actionName: '',
  };

  handlePaginationEvent(event: PaginationEvent) {
    const pageNumber = event.type === 'PageChanged' ? event.value + 1 : 1;
    const pageSize = event.type === 'PageSizeChanged' ? event.value : this.pageSize();
    this.#store.dispatch(new LoadRepositoriesGridItems(pageNumber, pageSize as 10 | 25 | 50 | 100 | 1000));
  }
}
