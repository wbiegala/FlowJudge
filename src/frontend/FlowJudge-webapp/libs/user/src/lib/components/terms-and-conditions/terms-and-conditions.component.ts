import { TranslatePipe } from '@ngx-translate/core';
import { MatButtonModule } from '@angular/material/button';
import { toObservable, toSignal } from '@angular/core/rxjs-interop';
import { ChangeDetectionStrategy, Component, inject, input, output, signal } from '@angular/core';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { LegalService } from '../../legal/legal.service';
import { switchMap } from 'rxjs';

@Component({
  selector: 'lib-terms-and-conditions',
  imports: [MatSlideToggleModule, MatButtonModule, TranslatePipe],
  templateUrl: './terms-and-conditions.component.html',
  styleUrl: './terms-and-conditions.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class TermsAndConditionsComponent {
  readonly termsVersion = input<'actual' | string>('actual');
  readonly isAcceptable = input(false);
  readonly termsAccepted = output<string>();
  #legalService = inject(LegalService);
  #data$ = toObservable(this.termsVersion).pipe(
    switchMap(version =>
      version === 'actual'
        ? this.#legalService.getActualTermsAndConditions()
        : this.#legalService.getTermsAndConditions(version)
    )
  );

  readonly data = toSignal(this.#data$, { initialValue: undefined });
  readonly isAcceptance = signal(false);

  onAcceptClick() {
    const isAcceptable = this.isAcceptable();
    const data = this.data();

    if (isAcceptable && data && this.isAcceptance()) {
      this.termsAccepted.emit(data.versionId)
    }
  }

  onAcceptanceChanged(checked: boolean) {
    this.isAcceptance.set(checked);
  }
}
