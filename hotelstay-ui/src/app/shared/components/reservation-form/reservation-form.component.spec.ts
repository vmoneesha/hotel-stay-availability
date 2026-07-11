import { ComponentFixture, TestBed } from '@angular/core/testing';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { ReservationFormComponent } from './reservation-form.component';
import { HotelRoomDto, SelectedRoomContext } from '../../../models/hotel.models';

describe('ReservationFormComponent', () => {
  let fixture: ComponentFixture<ReservationFormComponent>;
  let component: ReservationFormComponent;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [NoopAnimationsModule, ReservationFormComponent]
    }).compileComponents();

    fixture = TestBed.createComponent(ReservationFormComponent);
    component = fixture.componentInstance;
  });

  it('defaults domestic reservations to National ID and accepts Passport as well', () => {
    // Arrange
    fixture.componentRef.setInput('selected', selectedContext('Hyderabad'));

    // Act
    fixture.detectChanges();
    component.form.controls.documentType.setValue('Passport');
    component.form.controls.documentNumber.setValue('P1234567');
    component.form.controls.guestName.setValue('Asha Rao');
    fixture.detectChanges();

    // Assert
    expect(component.isSelectedDestinationInternational()).toBeFalse();
    expect(component.form.controls.documentType.value).toBe('Passport');
    expect(component.form.valid).toBeTrue();
  });

  it('automatically selects Passport for an international destination', () => {
    // Arrange
    fixture.componentRef.setInput('selected', selectedContext('London'));

    // Act
    fixture.detectChanges();

    // Assert
    expect(component.isSelectedDestinationInternational()).toBeTrue();
    expect(component.form.controls.documentType.value).toBe('Passport');
    expect(component.hasDocumentTypeMismatch()).toBeFalse();
  });

  it('marks international National ID as invalid and disables document number entry', () => {
    // Arrange
    fixture.componentRef.setInput('selected', selectedContext('London'));
    fixture.detectChanges();

    // Act
    component.form.controls.documentType.setValue('NationalId');
    fixture.detectChanges();

    // Assert
    expect(component.hasDocumentTypeMismatch()).toBeTrue();
    expect(component.form.controls.documentNumber.disabled).toBeTrue();
    expect(fixture.nativeElement.textContent).toContain('Passport needed for London');
  });

  it('switches back to Passport and re-enables document number entry', () => {
    // Arrange
    fixture.componentRef.setInput('selected', selectedContext('London'));
    fixture.detectChanges();
    component.form.controls.documentType.setValue('NationalId');
    fixture.detectChanges();

    // Act
    component.usePassport();
    fixture.detectChanges();

    // Assert
    expect(component.form.controls.documentType.value).toBe('Passport');
    expect(component.form.controls.documentNumber.enabled).toBeTrue();
    expect(component.hasDocumentTypeMismatch()).toBeFalse();
  });

  it('does not emit reservation when required fields are missing', () => {
    // Arrange
    fixture.componentRef.setInput('selected', selectedContext('Hyderabad'));
    fixture.detectChanges();
    spyOn(component.reserve, 'emit');

    // Act
    component.submit();

    // Assert
    expect(component.reserve.emit).not.toHaveBeenCalled();
    expect(component.localErrors).toContain('Guest name is required.');
    expect(component.localErrors).toContain('Document number is required.');
  });

  it('emits a complete reservation request for valid form values', () => {
    // Arrange
    const selected = selectedContext('Hyderabad');
    fixture.componentRef.setInput('selected', selected);
    fixture.detectChanges();
    spyOn(component.reserve, 'emit');
    component.form.setValue({ guestName: 'Asha Rao', documentType: 'Passport', documentNumber: 'P1234567' });

    // Act
    component.submit();

    // Assert
    expect(component.reserve.emit).toHaveBeenCalledOnceWith(jasmine.objectContaining({
      destination: 'Hyderabad',
      providerCode: selected.room.providerCode,
      guestName: 'Asha Rao',
      documentType: 'Passport',
      documentNumber: 'P1234567'
    }));
  });

  function selectedContext(destination: string): SelectedRoomContext {
    return {
      room: room(destination),
      checkIn: '2026-08-10',
      checkOut: '2026-08-12'
    };
  }

  function room(destination: string): HotelRoomDto {
    return {
      providerCode: 'PremierStays',
      providerBadge: 'Premier',
      hotelId: `PS-${destination}`,
      hotelName: `Premier ${destination}`,
      destination,
      roomId: 'ROOM-1',
      roomType: 'Standard',
      perNightPrice: 100,
      totalStayPrice: 200,
      nights: 2,
      amenities: [],
      starRating: 5,
      cancellationPolicy: 'FreeCancellation',
      cancellationPolicyDescription: 'Free cancellation'
    };
  }
});
