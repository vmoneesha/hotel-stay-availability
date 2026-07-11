import { TestBed } from '@angular/core/testing';
import { HotelSelectionStore } from './hotel-selection.store';
import { HotelRoomDto } from '../models/hotel.models';

describe('HotelSelectionStore', () => {
  let store: HotelSelectionStore;
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
    TestBed.configureTestingModule({ providers: [HotelSelectionStore] });
    store = TestBed.inject(HotelSelectionStore);
  });

  it('stores and returns the selected room context', () => {
    // Arrange
    const context = { room, checkIn: '2026-08-10', checkOut: '2026-08-12' };

    // Act
    store.select(context);

    // Assert
    expect(store.current()).toEqual(context);
    expect(store.selectedRoom()).toEqual(context);
  });

  it('remembers rooms for confirmation references', () => {
    // Arrange
    const reference = 'HS-000001';

    // Act
    store.rememberConfirmation(reference, room);

    // Assert
    expect(store.confirmedRoom(reference)).toEqual(room);
    expect(store.confirmedRoom('missing')).toBeNull();
  });
});
