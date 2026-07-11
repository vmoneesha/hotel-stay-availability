import { Component, input, output } from '@angular/core';
import { HotelRoomDto } from '../../../models/hotel.models';
import { PriceCardComponent } from '../price-card/price-card.component';
import { ProviderBadgeComponent } from '../provider-badge/provider-badge.component';
import { RoomTypeBadgeComponent } from '../room-type-badge/room-type-badge.component';

@Component({
  selector: 'app-hotel-card',
  imports: [PriceCardComponent, ProviderBadgeComponent, RoomTypeBadgeComponent],
  templateUrl: './hotel-card.component.html',
  styleUrl: './hotel-card.component.scss'
})
export class HotelCardComponent {
  readonly room = input.required<HotelRoomDto>();
  readonly reserve = output<HotelRoomDto>();

  reserveRoom(): void {
    this.reserve.emit(this.room());
  }
}
