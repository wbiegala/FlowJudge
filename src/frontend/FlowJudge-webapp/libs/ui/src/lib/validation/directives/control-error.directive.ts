import {
  Directive,
  DestroyRef,
  ElementRef,
  Renderer2,
  effect,
  inject,
  input,
} from '@angular/core';
import { AbstractControl } from '@angular/forms';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { TranslateService } from '@ngx-translate/core';
import { merge, startWith } from 'rxjs';
import { ValidationErrorService } from '../validation-error.service';

@Directive({
  selector: 'mat-error[libControlError]',
  standalone: true,
})
export class ControlErrorDirective {
  private readonly elementRef = inject<ElementRef<HTMLElement>>(ElementRef);
  private readonly renderer = inject(Renderer2);
  private readonly destroyRef = inject(DestroyRef);
  private readonly translate = inject(TranslateService);
  private readonly validationErrors = inject(ValidationErrorService);

  readonly control = input.required<AbstractControl | null>();

  constructor() {
    effect((onCleanup) => {
      const control = this.control();

      if (!control) {
        this.setText('');
        return;
      }

      const subscription = merge(
        control.events,
        this.translate.onLangChange,
        this.translate.onTranslationChange,
      )
        .pipe(
          startWith(null),
          takeUntilDestroyed(this.destroyRef),
        )
        .subscribe(() => {
          this.updateErrorText(control);
        });

      onCleanup(() => {
        subscription.unsubscribe();
      });
    });
  }

  private updateErrorText(control: AbstractControl): void {
    if (!this.validationErrors.shouldShowError(control)) {
      this.setText('');
      return;
    }

    const error = this.validationErrors.getFirstError(control);

    if (!error) {
      this.setText('');
      return;
    }

    const text = this.translate.instant(error.key, error.params);

    this.setText(text);
  }

  private setText(text: string): void {
    this.renderer.setProperty(
      this.elementRef.nativeElement,
      'textContent',
      text,
    );
  }
}
