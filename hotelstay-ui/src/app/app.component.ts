import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { FooterComponent } from './shared/components/footer/footer.component';
import { HeaderComponent } from './shared/components/header/header.component';

@Component({
  selector: 'app-root',
  imports: [FooterComponent, HeaderComponent, RouterOutlet],
  template: `
    <app-header />
    <main class="shell-main">
      <router-outlet />
    </main>
    <app-footer />
  `
})
export class AppComponent {}