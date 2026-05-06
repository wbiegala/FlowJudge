import { ChangeDetectionStrategy, Component, input, output } from '@angular/core';
import {MatPaginatorModule, PageEvent} from '@angular/material/paginator';
import { PageSize, PaginationEvent } from '../../pagination.model';

@Component({
  selector: 'lib-pagination',
  imports: [ MatPaginatorModule ],
  templateUrl: './pagination.component.html',
  styleUrl: `./pagination.component.scss`,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class PaginationComponent {
  readonly pageSizes: Array<PageSize> = [ 10, 25, 50, 100, 1000 ];
  pageSize = input<PageSize>(25);
  itemsCount = input.required<number>();
  totalCount = input.required<number>();

  paginationEvent = output<PaginationEvent>();

  onPageEvent(event: PageEvent) {
    if (event.pageIndex === event.previousPageIndex) {
      this.paginationEvent.emit({ type: 'PageSizeChanged', value: event.pageSize });
    } else {
      this.paginationEvent.emit({ type: 'PageChanged', value: event.pageIndex });
    }
  }
}
