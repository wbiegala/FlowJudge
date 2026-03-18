import { ChangeDetectionStrategy, Component, output } from '@angular/core';
import { MatToolbarModule } from '@angular/material/toolbar';
import { HeaderProgressBarComponent } from './header-progress-bar/header-progress-bar.component';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { HeaderUserPanelComponent } from './header-user-panel/header-user-panel.component';

@Component({
  selector: 'app-header',
  imports: [HeaderProgressBarComponent, HeaderUserPanelComponent, MatToolbarModule, MatButtonModule, MatIconModule],
  templateUrl: './app-header.component.html',
  styleUrl: './app-header.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class AppHeaderComponent {
  menuClicked = output();
}
