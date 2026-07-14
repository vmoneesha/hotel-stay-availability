import { TestBed } from '@angular/core/testing';
import { provideRouter, Router, UrlTree } from '@angular/router';
import { HotelRoomDto } from '../models/hotel.models';
import { HotelSelectionStore } from '../services/hotel-selection.store';
import { reservationSelectionGuard } from './reservation-selection.guard';

describe('reservationSelectionGuard', () => {
  const room: HotelRoomDto = {
    providerCode: 'BudgetNests',
    providerBadge: 'Budget',
    hotelId: 'BN-HYDERABAD',
    hotelName: 'Budget Hyderabad Central',
    destination: 'Hyderabad',
    roomId: 'BN-HYD-STD',
    roomType: 'Standard',
    perNightPrice: 2900,
    totalStayPrice: 5800,
    nights: 2,
    amenities: [],
    starRating: null,
    cancellationPolicy: 'Flexible',
    cancellationPolicyDescription: 'Flexible cancellation'
  };

  beforeEach(() => {
    TestBed.configureTestingModule({ providers: [provideRouter([])] });
  });

  it('redirects to search when no room is selected', () => {
    // Arrange
    const router = TestBed.inject(Router);

    // Act
    const result = TestBed.runInInjectionContext(() => reservationSelectionGuard({} as never, {} as never));

    // Assert
    expect(result instanceof UrlTree).toBeTrue();
    expect(router.serializeUrl(result as UrlTree)).toBe('/search');
  });

  it('allows navigation when a room is selected', () => {
    // Arrange
    const selectionStore = TestBed.inject(HotelSelectionStore);
    selectionStore.select({ room, checkIn: '2026-08-10', checkOut: '2026-08-12' });

    // Act
    const result = TestBed.runInInjectionContext(() => reservationSelectionGuard({} as never, {} as never));

    // Assert
    expect(result).toBeTrue();
  });
});