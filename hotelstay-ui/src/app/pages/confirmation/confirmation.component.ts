import { Component, inject, signal } from '@angular/core';
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
export class ConfirmationComponent {
  private readonly route = inject(ActivatedRoute);
  private readonly hotelApi = inject(HotelApiService);
  private readonly selectionStore = inject(HotelSelectionStore);

  readonly reservation = signal<ReservationDto | null>(null);
  readonly room = signal<HotelRoomDto | null>(null);
  readonly loading = signal(true);
  readonly error = signal('');

  constructor() {
    this.loadReservation();
  }

  private loadReservation(): void {
    const reference = this.route.snapshot.paramMap.get('reference');
    if (!reference) {
      this.loading.set(false);
      this.error.set('Reservation reference is missing.');
      return;
    }

    this.hotelApi.getReservation(reference).subscribe({
      next: reservation => {
        this.reservation.set(reservation);
        this.room.set(this.selectionStore.confirmedRoom(reservation.reference));
        this.loading.set(false);
      },
      error: () => {
        this.loading.set(false);
        this.error.set('Reservation was not found.');
      }
    });
  }
}