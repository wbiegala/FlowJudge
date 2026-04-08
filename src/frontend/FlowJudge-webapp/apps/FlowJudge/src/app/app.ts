import { Component, inject } from '@angular/core';
import { RouterModule } from '@angular/router';
import { LegalStateWatcherService } from '@flow-judge-webapp/user';

@Component({
  imports: [RouterModule],
  selector: 'app-root',
  templateUrl: './app.html',
  styleUrl: './app.scss',
})
export class App {
  protected title = 'FlowJudge';
  private readonly legalStateWatcher = inject(LegalStateWatcherService);

  constructor() {
    this.legalStateWatcher.init();
  }
}
