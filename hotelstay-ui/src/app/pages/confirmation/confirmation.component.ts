import { CurrencyPipe, DatePipe } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { ReservationDto } from '../../models/hotel.models';
import { HotelApiService } from '../../services/hotel-api.service';

@Component({
  selector: 'app-confirmation',
  imports: [CurrencyPipe, DatePipe, RouterLink],
  templateUrl: './confirmation.component.html'
})
export class ConfirmationComponent implements OnInit {
  reservation: ReservationDto | null = null;
  loading = true;
  error = '';

  constructor(
    private readonly route: ActivatedRoute,
    private readonly hotelApi: HotelApiService) {}

  ngOnInit(): void {
    const reference = this.route.snapshot.paramMap.get('reference');
    if (!reference) {
      this.loading = false;
      this.error = 'Reservation reference is missing.';
      return;
    }

    this.hotelApi.getReservation(reference).subscribe({
      next: reservation => {
        this.reservation = reservation;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
        this.error = 'Reservation was not found.';
      }
    });
  }
}