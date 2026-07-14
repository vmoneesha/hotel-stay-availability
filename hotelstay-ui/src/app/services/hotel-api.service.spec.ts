import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { HotelApiService } from './hotel-api.service';
import { HotelRoomDto, ReservationDto, ReserveRoomRequest } from '../models/hotel.models';
import { environment } from '../../environments/environment';

describe('HotelApiService', () => {
  let service: HotelApiService;
  let httpMock: HttpTestingController;

  const room: HotelRoomDto = {
    providerCode: 'PremierStays',
    providerBadge: 'Premier',
    hotelId: 'PS-LON-010',
    hotelName: 'Premier London Regent',
    destination: 'London',
    roomId: 'PS-LON-DLX',
    roomType: 'Deluxe',
    perNightPrice: 185,
    totalStayPrice: 370,
    nights: 2,
    amenities: ['Breakfast'],
    starRating: 5,
    cancellationPolicy: 'Refundable',
    cancellationPolicyDescription: 'Refundable until 72 hours before check-in'
  };

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting(), HotelApiService]
    });

    service = TestBed.inject(HotelApiService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('maps a raw array search response into the frontend search response shape', () => {
    // Arrange
    let responseRooms: HotelRoomDto[] = [];

    // Act
    service.search('London', '2026-08-10', '2026-08-12', '').subscribe(response => {
      responseRooms = response.rooms;
      expect(response.destination).toBe('London');
      expect(response.checkIn).toBe('2026-08-10');
      expect(response.checkOut).toBe('2026-08-12');
    });

    const request = httpMock.expectOne(req => req.url === `${environment.apiBaseUrl}/hotels/search`);
    request.flush([room]);

    // Assert
    expect(request.request.method).toBe('GET');
    expect(request.request.params.get('destination')).toBe('London');
    expect(request.request.params.get('checkIn')).toBe('2026-08-10');
    expect(request.request.params.get('checkOut')).toBe('2026-08-12');
    expect(request.request.params.has('roomType')).toBeFalse();
    expect(responseRooms).toEqual([room]);
  });

  it('sends the selected room type when searching with a room filter', () => {
    // Arrange
    let rooms: HotelRoomDto[] = [];

    // Act
    service.search('London', '2026-08-10', '2026-08-12', 'Deluxe').subscribe(response => rooms = response.rooms);

    const request = httpMock.expectOne(req => req.url === `${environment.apiBaseUrl}/hotels/search`);
  request.flush([room]);

    // Assert
    expect(request.request.params.get('roomType')).toBe('Deluxe');
    expect(rooms).toEqual([room]);
  });

  it('posts a reservation request and returns the confirmation', () => {
    // Arrange
    const reservationRequest: ReserveRoomRequest = {
      destination: 'Hyderabad',
      checkIn: '2026-08-10',
      checkOut: '2026-08-11',
      providerCode: 'BudgetNests',
      hotelId: 'BN-HYDERABAD',
      roomId: 'BN-HYD-STD',
      roomType: 'Standard',
      guestName: 'Asha Rao',
      documentType: 'Passport',
      documentNumber: 'P1234567',
      perNightPrice: 2900
    };
    const confirmation: ReservationDto = {
      ...reservationRequest,
      reference: 'HS-000001',
      totalStayPrice: 2900,
      nights: 1,
      createdAtUtc: '2026-07-11T00:00:00Z'
    };
    let actual: ReservationDto | undefined;

    // Act
    service.reserve(reservationRequest).subscribe(response => actual = response);

    const request = httpMock.expectOne(`${environment.apiBaseUrl}/hotels/reserve`);
    request.flush(confirmation);

    // Assert
    expect(request.request.method).toBe('POST');
    expect(request.request.body).toEqual(reservationRequest);
    expect(actual).toEqual(confirmation);
  });

  it('surfaces reservation lookup errors from the API', () => {
    // Arrange
    let status = 0;

    // Act
    service.getReservation('missing').subscribe({
      error: error => status = error.status
    });

    const request = httpMock.expectOne(`${environment.apiBaseUrl}/hotels/reservation/missing`);
    request.flush({ error: 'NotFound' }, { status: 404, statusText: 'Not Found' });

    // Assert
    expect(request.request.method).toBe('GET');
    expect(status).toBe(404);
  });
});
