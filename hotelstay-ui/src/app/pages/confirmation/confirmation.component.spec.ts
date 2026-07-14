import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ActivatedRoute } from '@angular/router';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { of, throwError } from 'rxjs';
import { ConfirmationComponent } from './confirmation.component';
import { HotelRoomDto, ReservationDto } from '../../models/hotel.models';
import { HotelApiService } from '../../services/hotel-api.service';
import { HotelSelectionStore } from '../../services/hotel-selection.store';

describe('ConfirmationComponent', () => {
  let fixture: ComponentFixture<ConfirmationComponent>;
  let component: ConfirmationComponent;
  let hotelApi: jasmine.SpyObj<HotelApiService>;
  let selectionStore: HotelSelectionStore;
  let routeReference: string | null;

  beforeEach(async () => {
    hotelApi = jasmine.createSpyObj<HotelApiService>('HotelApiService', ['getReservation']);
    routeReference = 'HS-000010';

    await TestBed.configureTestingModule({
      imports: [NoopAnimationsModule, ConfirmationComponent],
      providers: [
        HotelSelectionStore,
        { provide: HotelApiService, useValue: hotelApi },
        {
          provide: ActivatedRoute,
          useValue: {
            snapshot: {
              paramMap: {
                get: () => routeReference
              }
            }
          }
        }
      ]
    }).compileComponents();

    selectionStore = TestBed.inject(HotelSelectionStore);
  });

  it('loads a reservation and displays the remembered room details', () => {
    // Arrange
    const confirmation = reservation('HS-000010');
    const selectedRoom = room();
    selectionStore.rememberConfirmation('HS-000010', selectedRoom);
    hotelApi.getReservation.and.returnValue(of(confirmation));

    // Act
    createComponent();
    fixture.detectChanges();

    // Assert
    expect(hotelApi.getReservation).toHaveBeenCalledOnceWith('HS-000010');
    expect(component.reservation()).toEqual(confirmation);
    expect(component.room()).toEqual(selectedRoom);
    expect(component.loading()).toBeFalse();
  });

  it('shows an error when the reservation reference is missing', () => {
    // Arrange
    routeReference = null;

    // Act
    createComponent();
    fixture.detectChanges();

    // Assert
    expect(component.loading()).toBeFalse();
    expect(component.error()).toBe('Reservation reference is missing.');
    expect(hotelApi.getReservation).not.toHaveBeenCalled();
  });

  it('shows a not-found message when the API lookup fails', () => {
    // Arrange
    hotelApi.getReservation.and.returnValue(throwError(() => ({ status: 404 })));

    // Act
    createComponent();
    fixture.detectChanges();

    // Assert
    expect(component.loading()).toBeFalse();
    expect(component.error()).toBe('Reservation was not found.');
    expect(component.reservation()).toBeNull();
  });

  function createComponent(): void {
    fixture = TestBed.createComponent(ConfirmationComponent);
    component = fixture.componentInstance;
  }

  function reservation(reference: string): ReservationDto {
    return {
      reference,
      destination: 'Hyderabad',
      checkIn: '2026-08-10',
      checkOut: '2026-08-12',
      providerCode: 'PremierStays',
      hotelId: 'PS-HYD-001',
      roomId: 'PS-HYD-STD',
      roomType: 'Standard',
      guestName: 'Asha Rao',
      documentType: 'Passport',
      documentNumber: 'P1234567',
      perNightPrice: 100,
      totalStayPrice: 200,
      nights: 2,
      createdAtUtc: '2026-07-11T00:00:00Z'
    };
  }

  function room(): HotelRoomDto {
    return {
      providerCode: 'PremierStays',
      providerBadge: 'Premier',
      hotelId: 'PS-HYD-001',
      hotelName: 'Premier Hyderabad Skyline',
      destination: 'Hyderabad',
      roomId: 'PS-HYD-STD',
      roomType: 'Standard',
      perNightPrice: 100,
      totalStayPrice: 200,
      nights: 2,
      amenities: ['Breakfast'],
      starRating: 5,
      cancellationPolicy: 'FreeCancellation',
      cancellationPolicyDescription: 'Free cancellation'
    };
  }
});
