import { Component, effect, input, output } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { AbstractControl, FormBuilder, FormControl, FormGroup, ReactiveFormsModule, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { DocumentType, ReserveRoomRequest, SelectedRoomContext } from '../../../models/hotel.models';
import { documentLabel } from '../../../validation/document-rules';
import { ErrorMessageComponent } from '../error-message/error-message.component';

type ReservationForm = FormGroup<{
  guestName: FormControl<string>;
  documentType: FormControl<DocumentType>;
  documentNumber: FormControl<string>;
}>;

const internationalDestinations = new Set(['London', 'Dubai', 'Singapore']);

@Component({
  selector: 'app-reservation-form',
  imports: [ErrorMessageComponent, MatButtonModule, ReactiveFormsModule],
  templateUrl: './reservation-form.component.html',
  styleUrl: './reservation-form.component.scss'
})
export class ReservationFormComponent {
  readonly selected = input<SelectedRoomContext | null>(null);
  readonly loading = input(false);
  readonly serverErrors = input<readonly string[]>([]);
  readonly reserve = output<ReserveRoomRequest>();
  readonly documentTypes: DocumentType[] = ['NationalId', 'Passport'];
  readonly form: ReservationForm;
  submitted = false;
  localErrors: string[] = [];

  constructor(formBuilder: FormBuilder) {
    this.form = formBuilder.nonNullable.group({
      guestName: ['', Validators.required],
      documentType: ['NationalId' as DocumentType, [Validators.required, this.documentTypeValidator()]],
      documentNumber: ['', Validators.required]
    });

    effect(() => {
      const selected = this.selected();
      if (!selected) {
        return;
      }

      const documentTypeControl = this.form.controls.documentType;

      if (this.isInternationalDestination(selected.room.destination)) {
        documentTypeControl.setValue('Passport', { emitEvent: false });
      }

      documentTypeControl.enable({ emitEvent: false });
      documentTypeControl.updateValueAndValidity({ emitEvent: false });
      this.syncDocumentNumberAvailability();
      this.localErrors = [];
    });

    this.form.controls.documentType.valueChanges.subscribe(() => {
      this.form.controls.documentType.updateValueAndValidity({ emitEvent: false });
      this.syncDocumentNumberAvailability();
      this.localErrors = [];
    });
  }

  submit(): void {
    this.submitted = true;
    this.form.markAllAsTouched();
    this.localErrors = this.validationErrors();
    const selected = this.selected();
    if (!selected || this.form.invalid || this.localErrors.length > 0) {
      return;
    }

    const value = this.form.getRawValue();
    this.reserve.emit({
      destination: selected.room.destination,
      checkIn: selected.checkIn,
      checkOut: selected.checkOut,
      providerCode: selected.room.providerCode,
      hotelId: selected.room.hotelId,
      roomId: selected.room.roomId,
      roomType: selected.room.roomType,
      guestName: value.guestName,
      documentType: value.documentType,
      documentNumber: value.documentNumber,
      perNightPrice: selected.room.perNightPrice
    });
  }

  labelFor(documentType: DocumentType): string {
    return documentLabel(documentType);
  }

  documentMessage(): string {
    return this.isSelectedDestinationInternational()
      ? `${this.selectedDestination()} requires Passport for confirmed reservations. You can review National ID, but switch to Passport before continuing.`
      : 'Domestic destinations accept either National ID or Passport. National ID is selected by default.';
  }

  selectedDestination(): string {
    return this.selected()?.room.destination ?? 'This destination';
  }

  hasDocumentTypeMismatch(): boolean {
    return this.form.controls.documentType.hasError('passportRequired');
  }

  isDocumentNumberDisabled(): boolean {
    return this.form.controls.documentNumber.disabled;
  }

  usePassport(): void {
    this.form.controls.documentType.setValue('Passport');
    this.form.controls.documentType.markAsDirty();
    this.form.controls.documentType.markAsTouched();
    this.form.controls.documentType.updateValueAndValidity();
    this.localErrors = [];
  }

  confirmButtonLabel(): string {
    if (this.loading()) {
      return 'Confirming...';
    }

    return this.hasDocumentTypeMismatch() ? 'Select Passport to continue' : 'Confirm reservation';
  }

  isSelectedDestinationInternational(): boolean {
    const selected = this.selected();
    return selected ? this.isInternationalDestination(selected.room.destination) : false;
  }

  showError(controlName: 'guestName' | 'documentType' | 'documentNumber'): boolean {
    const control = this.form.controls[controlName];
    return control.invalid && (this.submitted || control.touched || control.dirty);
  }

  isInternationalDestination(destination: string): boolean {
    return internationalDestinations.has(destination);
  }

  private validationErrors(): string[] {
    const selected = this.selected();
    if (!selected) {
      return ['Select a room before confirming a reservation.'];
    }

    const value = this.form.getRawValue();
    const errors: string[] = [];

    if (!value.guestName.trim()) {
      errors.push('Guest name is required.');
    }

    if (!this.hasDocumentTypeMismatch() && !value.documentNumber.trim()) {
      errors.push('Document number is required.');
    }

    if (this.isInternationalDestination(selected.room.destination) && value.documentType !== 'Passport') {
      errors.push(`${selected.room.destination} requires Passport. Switch the document type to Passport to continue.`);
    }

    return errors;
  }

  private documentTypeValidator(): ValidatorFn {
    return (control: AbstractControl<DocumentType>): ValidationErrors | null => {
      const selected = this.selected();
      return selected && this.isInternationalDestination(selected.room.destination) && control.value !== 'Passport'
        ? { passportRequired: true }
        : null;
    };
  }

  private syncDocumentNumberAvailability(): void {
    const documentNumberControl = this.form.controls.documentNumber;

    if (this.hasDocumentTypeMismatch()) {
      documentNumberControl.reset('', { emitEvent: false });
      documentNumberControl.disable({ emitEvent: false });
      return;
    }

    documentNumberControl.enable({ emitEvent: false });
  }
}
