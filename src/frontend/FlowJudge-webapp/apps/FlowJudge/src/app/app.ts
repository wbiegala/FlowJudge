import { Component, inject } from '@angular/core';
import { RouterModule } from '@angular/router';
import { LegalStateWatcherService } from '@flow-judge-webapp/user';
import { WorkspaceContextUrlWatcherService } from '@flow-judge-webapp/workspaces';

@Component({
  imports: [RouterModule],
  selector: 'app-root',
  templateUrl: './app.html',
  styleUrl: './app.scss',
})
export class App {
  protected title = 'FlowJudge';
  private readonly legalStateWatcher = inject(LegalStateWatcherService);
  private readonly workspaceContextUrlWatcher = inject(WorkspaceContextUrlWatcherService);

  constructor() {
    this.legalStateWatcher.init();
    this.workspaceContextUrlWatcher.init();
  }
}
