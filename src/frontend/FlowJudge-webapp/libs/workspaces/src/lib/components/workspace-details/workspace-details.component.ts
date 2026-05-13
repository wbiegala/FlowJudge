import { ChangeDetectionStrategy, ChangeDetectorRef, Component, computed, inject } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { NgxsFormDirective } from '@ngxs/form-plugin';
import { ControlErrorDirective, FormSectionComponent, getViewModeTranslationKey, ViewHeaderAction, ViewHeaderComponent, ViewHeaderEvent } from '@flow-judge-webapp/ui';
import { TranslatePipe } from '@ngx-translate/core';
import { Navigate } from '@ngxs/router-plugin';
import { Store } from '@ngxs/store';
import { WorkspaceState } from '../../store/workspace/workspace.state';
import { CleanWorkspace, SaveWorkspace } from '../../store/workspace/workspace.actions';

@Component({
  selector: 'lib-workspace-details',
  imports: [ ViewHeaderComponent, FormSectionComponent, ControlErrorDirective, ReactiveFormsModule, TranslatePipe, NgxsFormDirective, MatInputModule, MatFormFieldModule ],
  templateUrl: './workspace-details.component.html',
  styleUrl: './workspace-details.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class WorkspaceDetailsComponent {
  #store = inject(Store);
  #cdr = inject(ChangeDetectorRef);
  viewMode = this.#store.selectSignal(WorkspaceState.viewMode);
  viewModeKey = computed(() => getViewModeTranslationKey(this.viewMode()));

  basicDataForm = new FormGroup({
    name: new FormControl('', { nonNullable: true, validators: [Validators.required, Validators.maxLength(128)] }),
  });

  readonly actions: Array<ViewHeaderAction> = [
    {
      name: 'save',
      nameTranslationKey: 'WORKSPACES.DETAILS.ACTIONS.SAVE',
      icon: 'check',
      canExecute: () => this.viewMode() !== 'Preview'
    },
    {
      name: 'exit',
      nameTranslationKey: 'WORKSPACES.DETAILS.ACTIONS.EXIT',
      icon: 'close',
      canExecute: () => true
    }
  ];

  handleActionEvent(event: ViewHeaderEvent) {
    switch (event.actionName) {
      case 'save': this.#handleSaveAction(); break;
      case 'exit': this.#handleExitAction(); break;
    }
  }

  #handleSaveAction() {
    this.basicDataForm.markAllAsTouched();
    this.basicDataForm.updateValueAndValidity({ emitEvent: true });
    this.#cdr.markForCheck();
    const nameError = this.basicDataForm.controls.name.errors;
    console.log(nameError);

    if (this.basicDataForm.invalid) {
      return;
    }

    this.#store.dispatch(new SaveWorkspace()).subscribe(_ => this.#handleExitAction());
  }

  #handleExitAction() {
    this.#store.dispatch([new CleanWorkspace(), new Navigate(['workspaces'])]);

  }
}
