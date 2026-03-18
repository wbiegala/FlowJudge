import { ChangeDetectionStrategy, Component } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'app-header-user-panel',
  imports: [MatButtonModule],
  templateUrl: './header-user-panel.component.html',
  styleUrl: './header-user-panel.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class HeaderUserPanelComponent {}
