import { Component } from '@angular/core';
import { RouterLink, RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-root',
  imports: [RouterLink, RouterOutlet],
  template: `
    <header class="shell-header">
      <a routerLink="/search" class="brand">Hotel Stay Availability</a>
      <nav aria-label="Primary navigation">
        <a routerLink="/search">Search</a>
      </nav>
    </header>
    <main class="shell-main">
      <router-outlet />
    </main>
  `
})
export class AppComponent {}