import { Component, OnInit } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { HotelRoomDto, ReservationDto } from '../../models/hotel.models';
import { HotelApiService } from '../../services/hotel-api.service';
import { HotelSelectionStore } from '../../services/hotel-selection.store';
import { BookingProgressComponent } from '../../shared/components/booking-progress/booking-progress.component';
import { ConfirmationCardComponent } from '../../shared/components/confirmation-card/confirmation-card.component';
import { EmptyStateComponent } from '../../shared/components/empty-state/empty-state.component';
import { LoadingSpinnerComponent } from '../../shared/components/loading-spinner/loading-spinner.component';

@Component({
  selector: 'app-confirmation',
  imports: [BookingProgressComponent, ConfirmationCardComponent, EmptyStateComponent, LoadingSpinnerComponent, MatButtonModule, RouterLink],
  templateUrl: './confirmation.component.html',
  styleUrl: './confirmation.component.scss'
})
export class ConfirmationComponent implements OnInit {
  reservation: ReservationDto | null = null;
  room: HotelRoomDto | null = null;
  loading = true;
  error = '';

  constructor(
    private readonly route: ActivatedRoute,
    private readonly hotelApi: HotelApiService,
    private readonly selectionStore: HotelSelectionStore) {}

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
        this.room = this.selectionStore.confirmedRoom(reservation.reference);
        this.loading = false;
      },
      error: () => {
        this.loading = false;
        this.error = 'Reservation was not found.';
      }
    });
  }
}