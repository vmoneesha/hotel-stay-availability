import { AsyncPipe, CurrencyPipe } from '@angular/common';
import { Component } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { finalize } from 'rxjs';
import { DocumentType, ReserveRoomRequest, ValidationProblemResponse } from '../../models/hotel.models';
import { HotelApiService } from '../../services/hotel-api.service';
import { HotelSelectionStore } from '../../services/hotel-selection.store';
import { documentLabel, requiredDocumentFor } from '../../validation/document-rules';

@Component({
  selector: 'app-reservation',
  imports: [AsyncPipe, CurrencyPipe, ReactiveFormsModule, RouterLink],
  templateUrl: './reservation.component.html'
})
export class ReservationComponent {
  readonly selectedRoom$;
  readonly documentTypes: DocumentType[] = ['NationalId', 'Passport'];
  readonly form;

  validationErrors: string[] = [];
  loading = false;

  constructor(
    formBuilder: FormBuilder,
    private readonly hotelApi: HotelApiService,
    private readonly selectionStore: HotelSelectionStore,
    private readonly router: Router) {
    this.selectedRoom$ = this.selectionStore.selectedRoom$;
    this.form = formBuilder.nonNullable.group({
      guestName: ['', Validators.required],
      documentType: ['NationalId' as DocumentType, Validators.required],
      documentNumber: ['', Validators.required]
    });
  }

  reserve(): void {
    const selected = this.selectionStore.current();
    if (!selected) {
      this.validationErrors = ['Select a room before confirming a reservation.'];
      return;
    }

    const value = this.form.getRawValue();
    const requiredDocument = requiredDocumentFor(selected.room.destination);
    this.validationErrors = [];

    if (!value.guestName) {
      this.validationErrors.push('Guest name is required.');
    }

    if (!value.documentNumber) {
      this.validationErrors.push('Document number is required.');
    }

    if (value.documentType !== requiredDocument) {
      this.validationErrors.push(`${selected.room.destination} requires ${documentLabel(requiredDocument)}.`);
    }

    if (this.validationErrors.length > 0) {
      return;
    }

    const request: ReserveRoomRequest = {
      destination: selected.room.destination,
      checkIn: selected.checkIn,
      checkOut: selected.checkOut,
      provider: selected.room.provider,
      hotelId: selected.room.hotelId,
      roomId: selected.room.roomId,
      roomType: selected.room.roomType,
      guestName: value.guestName,
      documentType: value.documentType,
      documentNumber: value.documentNumber,
      perNightPrice: selected.room.perNightPrice
    };

    this.loading = true;
    this.hotelApi.reserve(request)
      .pipe(finalize(() => this.loading = false))
      .subscribe({
        next: reservation => void this.router.navigate(['/confirmation', reservation.reference]),
        error: error => this.validationErrors = this.toErrorMessages(error.error)
      });
  }

  labelFor(documentType: DocumentType): string {
    return documentLabel(documentType);
  }

  private toErrorMessages(problem: ValidationProblemResponse | undefined): string[] {
    return problem?.details?.map(detail => detail.message) ?? ['Reservation failed.'];
  }
}