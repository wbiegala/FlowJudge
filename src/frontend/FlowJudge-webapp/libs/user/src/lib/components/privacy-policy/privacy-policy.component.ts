import { ChangeDetectionStrategy, Component, inject, input, output, signal } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { TranslatePipe } from '@ngx-translate/core';
import { LegalService } from '../../legal/legal.service';
import { toObservable, toSignal } from '@angular/core/rxjs-interop';
import { switchMap } from 'rxjs';
import { LoadingComponent } from '@flow-judge-webapp/ui';

@Component({
  selector: 'lib-privacy-policy',
  imports: [MatSlideToggleModule, MatButtonModule, TranslatePipe, LoadingComponent],
  templateUrl: './privacy-policy.component.html',
  styleUrl: './privacy-policy.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class PrivacyPolicyComponent {
  readonly privacyPolicyVersion = input<'actual' | string>('actual');
  readonly isAcceptable = input(false);
  readonly privacyPolicyAccepted = output<string>();
  #legalService = inject(LegalService);
  #data$ = toObservable(this.privacyPolicyVersion).pipe(
    switchMap(version =>
      version === 'actual'
        ? this.#legalService.getActualPrivacyPolicy()
        : this.#legalService.getPrivacyPolicy(version)
    )
  );

  readonly data = toSignal(this.#data$, { initialValue: undefined });
  readonly isAcceptance = signal(false);

  onAcceptClick() {
    const isAcceptable = this.isAcceptable();
    const data = this.data();

    if (isAcceptable && data && this.isAcceptance()) {
      this.privacyPolicyAccepted.emit(data.versionId)
    }
  }

  onAcceptanceChanged(checked: boolean) {
    this.isAcceptance.set(checked);
  }
}
