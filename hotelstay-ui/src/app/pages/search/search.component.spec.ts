import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Router } from '@angular/router';
import { of, throwError } from 'rxjs';
import { SearchComponent } from './search.component';
import { HotelRoomDto, HotelSearchResponse } from '../../models/hotel.models';
import { HotelApiService } from '../../services/hotel-api.service';
import { HotelSelectionStore } from '../../services/hotel-selection.store';

describe('SearchComponent', () => {
  let fixture: ComponentFixture<SearchComponent>;
  let component: SearchComponent;
  let hotelApi: jasmine.SpyObj<HotelApiService>;
  let router: jasmine.SpyObj<Router>;
  let selectionStore: HotelSelectionStore;

  const lowPriceRoom = room('BudgetNests', 'BN-HYD-STD', 'Standard', 2900);
  const highPriceRoom = room('PremierStays', 'PS-HYD-DLX', 'Deluxe', 8400);

  beforeEach(async () => {
    hotelApi = jasmine.createSpyObj<HotelApiService>('HotelApiService', ['search']);
    router = jasmine.createSpyObj<Router>('Router', ['navigate']);

    await TestBed.configureTestingModule({
      imports: [SearchComponent],
      providers: [
        HotelSelectionStore,
        { provide: HotelApiService, useValue: hotelApi },
        { provide: Router, useValue: router }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(SearchComponent);
    component = fixture.componentInstance;
    selectionStore = TestBed.inject(HotelSelectionStore);
    fixture.detectChanges();
  });

  it('loads rooms and clears the loading state after a successful search', () => {
    // Arrange
    const response: HotelSearchResponse = {
      destination: 'Hyderabad',
      checkIn: '2026-08-10',
      checkOut: '2026-08-12',
      rooms: [highPriceRoom, lowPriceRoom]
    };
    hotelApi.search.and.returnValue(of(response));

    // Act
    component.search({ destination: 'Hyderabad', checkIn: '2026-08-10', checkOut: '2026-08-12', roomType: '' });

    // Assert
    expect(hotelApi.search).toHaveBeenCalledWith('Hyderabad', '2026-08-10', '2026-08-12', '');
    expect(component.rooms()).toEqual([highPriceRoom, lowPriceRoom]);
    expect(component.loading()).toBeFalse();
    expect(component.searched()).toBeTrue();
  });

  it('shows validation messages from an API error response', () => {
    // Arrange
    hotelApi.search.and.returnValue(throwError(() => ({ error: { details: [{ field: 'destination', message: 'Destination is required.' }] } })));

    // Act
    component.search({ destination: '', checkIn: '2026-08-10', checkOut: '2026-08-12', roomType: '' });

    // Assert
    expect(component.rooms()).toEqual([]);
    expect(component.validationErrors()).toEqual(['Destination is required.']);
    expect(component.loading()).toBeFalse();
  });

  it('sorts rooms by total price by default and by room type when requested', () => {
    // Arrange
    component.rooms.set([highPriceRoom, lowPriceRoom]);

    // Act
    const byPrice = component.sortedRooms().map(room => room.roomId);
    component.updateSort('roomType');
    const byRoomType = component.sortedRooms().map(room => room.roomType);

    // Assert
    expect(byPrice).toEqual(['BN-HYD-STD', 'PS-HYD-DLX']);
    expect(byRoomType).toEqual(['Deluxe', 'Standard']);
  });

  it('stores the selected room and navigates to reservation after a search', () => {
    // Arrange
    component.lastSearch.set({ destination: 'Hyderabad', checkIn: '2026-08-10', checkOut: '2026-08-12', roomType: '' });

    // Act
    component.reserve(lowPriceRoom);

    // Assert
    expect(selectionStore.current()).toEqual({ room: lowPriceRoom, checkIn: '2026-08-10', checkOut: '2026-08-12' });
    expect(router.navigate).toHaveBeenCalledOnceWith(['/reservation']);
  });

  it('shows a guard message when reserve is clicked before searching', () => {
    // Arrange
    component.lastSearch.set(null);

    // Act
    component.reserve(lowPriceRoom);

    // Assert
    expect(component.validationErrors()).toEqual(['Search for stays before selecting a room.']);
    expect(router.navigate).not.toHaveBeenCalled();
  });

  function room(providerCode: string, roomId: string, roomType: 'Standard' | 'Deluxe', totalStayPrice: number): HotelRoomDto {
    return {
      providerCode,
      providerBadge: providerCode === 'BudgetNests' ? 'Budget' : 'Premier',
      hotelId: `${providerCode}-HOTEL`,
      hotelName: `${providerCode} Hotel`,
      destination: 'Hyderabad',
      roomId,
      roomType,
      perNightPrice: totalStayPrice / 2,
      totalStayPrice,
      nights: 2,
      amenities: [],
      starRating: null,
      cancellationPolicy: 'Flexible',
      cancellationPolicyDescription: 'Flexible cancellation'
    };
  }
});
