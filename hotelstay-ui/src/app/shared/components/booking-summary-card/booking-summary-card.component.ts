import { CurrencyPipe } from '@angular/common';
import { Component, input } from '@angular/core';
import { HotelRoomDto, SelectedRoomContext } from '../../../models/hotel.models';
import { ProviderBadgeComponent } from '../provider-badge/provider-badge.component';
import { RoomTypeBadgeComponent } from '../room-type-badge/room-type-badge.component';

@Component({
  selector: 'app-booking-summary-card',
  imports: [CurrencyPipe, ProviderBadgeComponent, RoomTypeBadgeComponent],
  templateUrl: './booking-summary-card.component.html',
  styleUrl: './booking-summary-card.component.scss'
})
export class BookingSummaryCardComponent {
  readonly selected = input.required<SelectedRoomContext>();
  readonly compact = input(false);

  room(): HotelRoomDto {
    return this.selected().room;
  }
}
