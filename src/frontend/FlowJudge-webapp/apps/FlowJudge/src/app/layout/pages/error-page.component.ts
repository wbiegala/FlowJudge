import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { ActivatedRoute } from '@angular/router';
import { ViewHeaderComponent } from '@flow-judge-webapp/ui';
import { TranslatePipe } from '@ngx-translate/core';
import { map } from 'rxjs';


@Component({
  selector: 'app-error-page',
  imports: [ TranslatePipe, ViewHeaderComponent ],
  template: `
<lib-view-header [title]="'ERRORS.HEADER' | translate">
  <p>{{ 'ERRORS.GENERIC' | translate }}</p>
  <p>{{ errorCode$() | translate }}</p>
  <p>{{ errorMessage$() | translate }}</p>
</lib-view-header>
  `,
  styles: `

  `,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ErrorPageComponent {
  #activatedRoute = inject(ActivatedRoute);
  errorCode$ = toSignal(this.#activatedRoute.queryParams.pipe(map(qp => qp['ErrorCode'])));
  errorMessage$ = toSignal(this.#activatedRoute.queryParams.pipe(map(qp => qp['ErrorMessage'])));

}
