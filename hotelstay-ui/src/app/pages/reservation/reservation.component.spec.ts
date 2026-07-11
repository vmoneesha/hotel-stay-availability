import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Router, provideRouter } from '@angular/router';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { of, throwError } from 'rxjs';
import { ReservationComponent } from './reservation.component';
import { HotelRoomDto, ReservationDto, ReserveRoomRequest } from '../../models/hotel.models';
import { HotelApiService } from '../../services/hotel-api.service';
import { HotelSelectionStore } from '../../services/hotel-selection.store';

describe('ReservationComponent', () => {
  let fixture: ComponentFixture<ReservationComponent>;
  let component: ReservationComponent;
  let hotelApi: jasmine.SpyObj<HotelApiService>;
  let router: Router;
  let selectionStore: HotelSelectionStore;

  beforeEach(async () => {
    hotelApi = jasmine.createSpyObj<HotelApiService>('HotelApiService', ['reserve']);

    await TestBed.configureTestingModule({
      imports: [NoopAnimationsModule, ReservationComponent],
      providers: [
        HotelSelectionStore,
        provideRouter([]),
        { provide: HotelApiService, useValue: hotelApi },
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(ReservationComponent);
    component = fixture.componentInstance;
    selectionStore = TestBed.inject(HotelSelectionStore);
    router = TestBed.inject(Router);
    spyOn(router, 'navigate');
    fixture.detectChanges();
  });

  it('shows a validation message when confirming without a selected room', () => {
    // Arrange
    const request = reserveRequest();

    // Act
    component.reserve(request);

    // Assert
    expect(component.validationErrors()).toEqual(['Select a room before confirming a reservation.']);
    expect(hotelApi.reserve).not.toHaveBeenCalled();
  });

  it('submits a reservation, remembers the room, and navigates to confirmation', () => {
    // Arrange
    const selectedRoom = room();
    const request = reserveRequest();
    const confirmation: ReservationDto = {
      ...request,
      reference: 'HS-000010',
      totalStayPrice: 200,
      nights: 2,
      createdAtUtc: '2026-07-11T00:00:00Z'
    };
    selectionStore.select({ room: selectedRoom, checkIn: request.checkIn, checkOut: request.checkOut });
    hotelApi.reserve.and.returnValue(of(confirmation));

    // Act
    component.reserve(request);

    // Assert
    expect(hotelApi.reserve).toHaveBeenCalledOnceWith(request);
    expect(selectionStore.confirmedRoom('HS-000010')).toEqual(selectedRoom);
    expect(router.navigate).toHaveBeenCalledOnceWith(['/confirmation', 'HS-000010']);
    expect(component.loading()).toBeFalse();
  });

  it('displays backend validation errors when reservation fails', () => {
    // Arrange
    const request = reserveRequest();
    selectionStore.select({ room: room(), checkIn: request.checkIn, checkOut: request.checkOut });
    hotelApi.reserve.and.returnValue(throwError(() => ({ error: { details: [{ field: 'documentType', message: 'London requires a valid Passport for reservation.' }] } })));

    // Act
    component.reserve(request);

    // Assert
    expect(component.validationErrors()).toEqual(['London requires a valid Passport for reservation.']);
    expect(component.loading()).toBeFalse();
    expect(router.navigate).not.toHaveBeenCalled();
  });

  function reserveRequest(): ReserveRoomRequest {
    return {
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
      perNightPrice: 100
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
      amenities: [],
      starRating: 5,
      cancellationPolicy: 'FreeCancellation',
      cancellationPolicyDescription: 'Free cancellation'
    };
  }
});
