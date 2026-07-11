import { Component, input } from '@angular/core';

@Component({
  selector: 'app-provider-badge',
  template: `<span class="provider-badge">{{ label() }}</span>`,
  styleUrl: './provider-badge.component.scss'
})
export class ProviderBadgeComponent {
  readonly label = input.required<string>();
}
