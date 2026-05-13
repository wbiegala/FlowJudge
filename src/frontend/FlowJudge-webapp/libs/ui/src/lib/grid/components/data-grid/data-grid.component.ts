import { ChangeDetectionStrategy, Component, computed, inject, input, output } from '@angular/core';
import { MatPaginatorModule } from '@angular/material/paginator'
import { MatSortModule } from '@angular/material/sort';
import { MatTableModule } from '@angular/material/table';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { MatButtonModule } from '@angular/material/button';
import { MatTooltipModule } from '@angular/material/tooltip';
import { TranslatePipe } from '@ngx-translate/core';
import { DataGridAction, DataGridActionEvent, DataGridColumn, DataGridRow, DataGridRowAction, DataGridRowActionEvent, EmptyGridBehavior } from '../../data-grid.model';
import { LoadingComponent } from '../../../progress/components/loading.component';
import { ViewportService } from '../../../viewport/viewport.service';

@Component({
  selector: 'lib-data-grid',
  imports: [LoadingComponent, MatTableModule, MatSortModule, MatPaginatorModule, MatButtonModule, MatMenuModule, MatIconModule, MatTooltipModule, TranslatePipe],
  templateUrl: './data-grid.component.html',
  styleUrl: './data-grid.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class DataGridComponent<TRow extends DataGridRow> {
  isViewportNarrow = inject(ViewportService).isNarrow;
  isLoading = input(false);
  gridActions = input<Array<DataGridAction>>([]);
  gridColumns = input.required<Array<DataGridColumn<TRow>>>();
  rows = input.required<Array<TRow>>();
  rowActions = input<Array<DataGridRowAction<TRow>>>([]);
  emptyGridBehavior = input<EmptyGridBehavior | null>(null);
  actionOnEmpty = computed(() => {
    const definedAction = this.emptyGridBehavior();
    if (definedAction !== null) {
      const actions = this.gridActions() ?? [];
      const canditates = actions.filter(a => a.canExecute() && a.name === definedAction.actionName);

      return canditates.length > 0 ? canditates[0] : null;
    }

    return null;
  });

  displayedColumns = computed(() => {
    let allColumns = this.gridColumns().filter(col => col.isVisible).map(col => col.id);
    const rowActions = this.rowActions();
    if (rowActions.length > 0) {
      allColumns = [...allColumns, 'row-actions'];
    }

    return allColumns;
  });

  gridActionEvent = output<DataGridActionEvent>();
  gridRowActionEvent = output<DataGridRowActionEvent>();

  onGridActionClick(action: string) {
    const event = {
      name: action
    } satisfies DataGridActionEvent;

    this.gridActionEvent.emit(event);
  }

  onRowActionClick(action: string, rowId: string) {
    const event = {
      name: action,
      id: rowId,
    } satisfies DataGridRowActionEvent;

    this.gridRowActionEvent.emit(event);
  }
}
