import { ChangeDetectionStrategy, Component, computed, effect, inject } from '@angular/core';
import { UserLegalService } from '../../legal/user-legal.service';
import { TermsAndConditionsComponent } from '../terms-and-conditions/terms-and-conditions.component';
import { PrivacyPolicyComponent } from '../privacy-policy/privacy-policy.component';
import { Router } from '@angular/router';
import { LoadingComponent } from '@flow-judge-webapp/ui';

@Component({
  selector: 'lib-legal-check-component',
  imports: [TermsAndConditionsComponent, PrivacyPolicyComponent, LoadingComponent],
  templateUrl: './legal-check.component.html',
  styleUrl: './legal-check.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class LegalCheckComponent {
  #router = inject(Router);
  #userLegalService = inject(UserLegalService);

  currentMissing = computed(() => {
    if (this.#userLegalService.isLegal()){
      return null;
    }

    const missings = this.#userLegalService.missings();
    return missings[0];
  })

  onTermsAccept(versionId: string) {
    this.#userLegalService.acceptTermsAndConditions(versionId).subscribe();
  }

  onPrivacyPolicyAccept(versionId: string) {
    this.#userLegalService.acceptPrivacyPolicy(versionId).subscribe();
  }

  redirectEffect = effect(() => {
    if (this.#userLegalService.isLegal()) {
      this.#router.navigateByUrl('/');
    }
  });
}
