import { Component, inject, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { finalize } from 'rxjs';
import { ReserveRoomRequest, ValidationProblemResponse } from '../../models/hotel.models';
import { HotelApiService } from '../../services/hotel-api.service';
import { HotelSelectionStore } from '../../services/hotel-selection.store';
import { BookingProgressComponent } from '../../shared/components/booking-progress/booking-progress.component';
import { BookingSummaryCardComponent } from '../../shared/components/booking-summary-card/booking-summary-card.component';
import { ReservationFormComponent } from '../../shared/components/reservation-form/reservation-form.component';

@Component({
  selector: 'app-reservation',
  imports: [BookingProgressComponent, BookingSummaryCardComponent, ReservationFormComponent, RouterLink],
  templateUrl: './reservation.component.html',
  styleUrl: './reservation.component.scss'
})
export class ReservationComponent {
  private readonly hotelApi = inject(HotelApiService);
  private readonly selectionStore = inject(HotelSelectionStore);
  private readonly router = inject(Router);

  readonly selectedRoom = this.selectionStore.selectedRoom;
  readonly validationErrors = signal<string[]>([]);
  readonly loading = signal(false);

  reserve(request: ReserveRoomRequest): void {
    const selected = this.selectionStore.current();
    if (!selected) {
      this.validationErrors.set(['Select a room before confirming a reservation.']);
      return;
    }

    this.validationErrors.set([]);
    this.loading.set(true);
    this.hotelApi.reserve(request)
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: reservation => {
          this.selectionStore.rememberConfirmation(reservation.reference, selected.room);
          void this.router.navigate(['/confirmation', reservation.reference]);
        },
        error: error => this.validationErrors.set(this.toErrorMessages(error.error))
      });
  }

  private toErrorMessages(problem: ValidationProblemResponse | undefined): string[] {
    return problem?.details?.map(detail => detail.message) ?? ['Reservation failed.'];
  }
}