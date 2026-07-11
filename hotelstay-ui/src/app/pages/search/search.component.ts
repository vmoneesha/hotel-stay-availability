import { CurrencyPipe } from '@angular/common';
import { Component } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { finalize } from 'rxjs';
import { HotelRoomDto, RoomType, ValidationProblemResponse } from '../../models/hotel.models';
import { HotelApiService } from '../../services/hotel-api.service';
import { HotelSelectionStore } from '../../services/hotel-selection.store';

@Component({
  selector: 'app-search',
  imports: [CurrencyPipe, ReactiveFormsModule],
  templateUrl: './search.component.html'
})
export class SearchComponent {
  private readonly formBuilder: FormBuilder;

  readonly destinations = ['Hyderabad', 'Bangalore', 'Mumbai', 'London', 'Dubai', 'Singapore'];
  readonly roomTypes: Array<RoomType | ''> = ['', 'Standard', 'Deluxe', 'Suite'];
  readonly form;

  rooms: HotelRoomDto[] = [];
  validationErrors: string[] = [];
  loading = false;
  searched = false;

  constructor(
    formBuilder: FormBuilder,
    private readonly hotelApi: HotelApiService,
    private readonly selectionStore: HotelSelectionStore,
    private readonly router: Router) {
    this.formBuilder = formBuilder;
    this.form = this.formBuilder.nonNullable.group({
    destination: ['', Validators.required],
    checkIn: ['', Validators.required],
    checkOut: ['', Validators.required],
    roomType: ['' as RoomType | '']
    });
  }

  search(): void {
    this.validationErrors = this.clientValidationErrors();
    this.searched = true;

    if (this.validationErrors.length > 0) {
      this.rooms = [];
      return;
    }

    const value = this.form.getRawValue();
    this.loading = true;
    this.hotelApi.search(value.destination, value.checkIn, value.checkOut, value.roomType)
      .pipe(finalize(() => this.loading = false))
      .subscribe({
        next: response => {
          this.rooms = [...response.rooms].sort((left, right) => left.totalStayPrice - right.totalStayPrice);
        },
        error: error => {
          this.rooms = [];
          this.validationErrors = this.toErrorMessages(error.error);
        }
      });
  }

  reserve(room: HotelRoomDto): void {
    const value = this.form.getRawValue();
    this.selectionStore.select({ room, checkIn: value.checkIn, checkOut: value.checkOut });
    void this.router.navigate(['/reservation']);
  }

  private clientValidationErrors(): string[] {
    const value = this.form.getRawValue();
    const errors: string[] = [];

    if (!value.destination) {
      errors.push('Destination is required.');
    }

    if (!value.checkIn) {
      errors.push('Check-in date is required.');
    }

    if (!value.checkOut) {
      errors.push('Check-out date is required.');
    }

    if (value.checkIn && value.checkOut && value.checkOut <= value.checkIn) {
      errors.push('Check-out date must be after check-in date.');
    }

    return errors;
  }

  private toErrorMessages(problem: ValidationProblemResponse | undefined): string[] {
    return problem?.details?.map(detail => detail.message) ?? ['Search failed.'];
  }
}