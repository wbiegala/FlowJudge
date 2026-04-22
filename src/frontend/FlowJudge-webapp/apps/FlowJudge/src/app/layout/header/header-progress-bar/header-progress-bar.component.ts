import { ChangeDetectionStrategy, Component, computed, inject } from '@angular/core';
import { MatProgressBarModule, ProgressBarMode } from '@angular/material/progress-bar';
import { ProgressService } from '@flow-judge-webapp/ui';

@Component({
  selector: 'app-header-progress-bar',
  imports: [MatProgressBarModule],
  templateUrl: './header-progress-bar.component.html',
  styleUrl: './header-progress-bar.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class HeaderProgressBarComponent {
  barMode = computed<ProgressBarMode>(() => this.#isActive() ? 'indeterminate' : 'determinate');
  barValue = computed<number | null>(() => this.#isActive() ? null : 0);
  #progressService = inject(ProgressService);

  #isActive = computed(() => {
    return this.#progressService.isProgress();
  });
}
