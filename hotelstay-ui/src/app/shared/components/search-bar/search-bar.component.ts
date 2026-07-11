import { Component, signal, output } from '@angular/core';
import { AbstractControl, FormBuilder, FormControl, FormGroup, ReactiveFormsModule, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { RoomType, SearchFormValue } from '../../../models/hotel.models';

type SearchBarForm = FormGroup<{
  destination: FormControl<string>;
  checkIn: FormControl<string>;
  checkOut: FormControl<string>;
  roomType: FormControl<RoomType | ''>;
}>;

@Component({
  selector: 'app-search-bar',
  imports: [ReactiveFormsModule],
  templateUrl: './search-bar.component.html',
  styleUrl: './search-bar.component.scss'
})
export class SearchBarComponent {
  readonly search = output<SearchFormValue>();
  readonly destinations = ['Hyderabad', 'Bangalore', 'Mumbai', 'London', 'Dubai', 'Singapore'];
  readonly roomTypes: Array<RoomType | ''> = ['', 'Standard', 'Deluxe', 'Suite'];
  readonly todayDate = this.toDateInputValue(new Date());
  readonly minCheckOutDate = signal(this.addDays(this.todayDate, 1));
  readonly form: SearchBarForm;
  submitted = false;
  checkOutWasCleared = false;

  constructor(formBuilder: FormBuilder) {
    this.form = formBuilder.nonNullable.group({
      destination: ['', Validators.required],
      checkIn: [this.todayDate, [Validators.required, this.minDateValidator(this.todayDate)]],
      checkOut: [this.minCheckOutDate(), [Validators.required, this.checkOutAfterCheckInValidator()]],
      roomType: ['' as RoomType | '']
    });

    this.form.controls.checkIn.valueChanges.subscribe(checkIn => {
      this.checkOutWasCleared = false;

      if (checkIn && this.compareDateOnly(checkIn, this.todayDate) < 0) {
        this.form.controls.checkIn.setValue(this.todayDate);
        return;
      }

      const nextMinCheckOut = checkIn ? this.addDays(checkIn, 1) : this.addDays(this.todayDate, 1);
      this.minCheckOutDate.set(nextMinCheckOut);

      const checkOut = this.form.controls.checkOut.value;
      if (checkOut && this.compareDateOnly(checkOut, nextMinCheckOut) < 0) {
        this.form.controls.checkOut.setValue('');
        this.checkOutWasCleared = true;
      }

      this.form.controls.checkOut.updateValueAndValidity({ emitEvent: false });
    });
  }

  submit(): void {
    this.submitted = true;
    this.form.markAllAsTouched();
    if (this.form.invalid) {
      return;
    }

    this.search.emit(this.form.getRawValue());
  }

  hasInvalidDateRange(): boolean {
    return this.form.controls.checkOut.hasError('checkOutBeforeMinimum');
  }

  showError(controlName: 'destination' | 'checkIn' | 'checkOut'): boolean {
    const control = this.form.controls[controlName];
    return control.invalid && (this.submitted || control.touched || control.dirty);
  }

  isInvalid(controlName: 'destination' | 'checkIn' | 'checkOut'): boolean {
    return this.showError(controlName);
  }

  normalizeDateInput(controlName: 'checkIn' | 'checkOut'): void {
    const control = this.form.controls[controlName];
    const value = control.value;
    if (!value) {
      control.markAsTouched();
      return;
    }

    if (controlName === 'checkIn' && this.compareDateOnly(value, this.todayDate) < 0) {
      control.setValue(this.todayDate);
      return;
    }

    if (controlName === 'checkOut' && this.compareDateOnly(value, this.minCheckOutDate()) < 0) {
      control.setValue('');
      this.checkOutWasCleared = true;
    }
  }

  checkInErrorMessage(): string {
    const control = this.form.controls.checkIn;
    if (control.hasError('required')) {
      return 'Check-in date is required.';
    }

    if (control.hasError('dateBeforeMinimum')) {
      return 'Check-in cannot be before today.';
    }

    return 'Enter a valid check-in date.';
  }

  checkOutErrorMessage(): string {
    const control = this.form.controls.checkOut;
    if (control.hasError('required')) {
      return this.checkOutWasCleared
        ? 'Select a new check-out date after the updated check-in date.'
        : 'Check-out date is required.';
    }

    if (control.hasError('checkOutBeforeMinimum')) {
      return 'Check-out must be at least one day after check-in.';
    }

    return 'Enter a valid check-out date.';
  }

  private minDateValidator(minDate: string): ValidatorFn {
    return (control: AbstractControl<string>): ValidationErrors | null => {
      return control.value && this.compareDateOnly(control.value, minDate) < 0
        ? { dateBeforeMinimum: true }
        : null;
    };
  }

  private checkOutAfterCheckInValidator(): ValidatorFn {
    return (control: AbstractControl<string>): ValidationErrors | null => {
      return control.value && this.compareDateOnly(control.value, this.minCheckOutDate()) < 0
        ? { checkOutBeforeMinimum: true }
        : null;
    };
  }

  private addDays(dateValue: string, days: number): string {
    const [year, month, day] = dateValue.split('-').map(Number);
    return this.toDateInputValue(new Date(year, month - 1, day + days));
  }

  private toDateInputValue(date: Date): string {
    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const day = String(date.getDate()).padStart(2, '0');
    return `${year}-${month}-${day}`;
  }

  private compareDateOnly(left: string, right: string): number {
    return left.localeCompare(right);
  }
}
