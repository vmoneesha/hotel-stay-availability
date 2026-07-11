import { CurrencyPipe } from '@angular/common';
import { Component, input } from '@angular/core';

@Component({
  selector: 'app-price-card',
  imports: [CurrencyPipe],
  templateUrl: './price-card.component.html',
  styleUrl: './price-card.component.scss'
})
export class PriceCardComponent {
  readonly perNightPrice = input.required<number>();
  readonly totalStayPrice = input.required<number>();
  readonly nights = input.required<number>();
}
