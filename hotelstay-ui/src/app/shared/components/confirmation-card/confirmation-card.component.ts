import { CurrencyPipe, DatePipe } from '@angular/common';
import { Component, input } from '@angular/core';
import { HotelRoomDto, ReservationDto } from '../../../models/hotel.models';

@Component({
  selector: 'app-confirmation-card',
  imports: [CurrencyPipe, DatePipe],
  templateUrl: './confirmation-card.component.html',
  styleUrl: './confirmation-card.component.scss'
})
export class ConfirmationCardComponent {
  readonly reservation = input.required<ReservationDto>();
  readonly room = input<HotelRoomDto | null>(null);
}
