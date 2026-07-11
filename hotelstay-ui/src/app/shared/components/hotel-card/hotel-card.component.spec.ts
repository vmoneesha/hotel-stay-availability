import { ComponentFixture, TestBed } from '@angular/core/testing';
import { HotelCardComponent } from './hotel-card.component';
import { HotelRoomDto } from '../../../models/hotel.models';

describe('HotelCardComponent', () => {
  let fixture: ComponentFixture<HotelCardComponent>;
  let component: HotelCardComponent;
  const room: HotelRoomDto = {
    providerCode: 'PremierStays',
    providerBadge: 'Premier',
    hotelId: 'PS-HYD-001',
    hotelName: 'Premier Hyderabad Skyline',
    destination: 'Hyderabad',
    roomId: 'PS-HYD-DLX',
    roomType: 'Deluxe',
    perNightPrice: 8400,
    totalStayPrice: 16800,
    nights: 2,
    amenities: ['Breakfast', 'Wi-Fi'],
    starRating: 5,
    cancellationPolicy: 'FreeCancellation',
    cancellationPolicyDescription: 'Free cancellation until 48 hours before check-in'
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [HotelCardComponent]
    }).compileComponents();

    fixture = TestBed.createComponent(HotelCardComponent);
    component = fixture.componentInstance;
    fixture.componentRef.setInput('room', room);
    fixture.detectChanges();
  });

  it('renders normalized hotel room details', () => {
    // Arrange
    const text = fixture.nativeElement.textContent as string;

    // Act and Assert
    expect(text).toContain('Premier Hyderabad Skyline');
    expect(text).toContain('Hyderabad');
    expect(text).toContain('Deluxe');
    expect(text).toContain('Breakfast');
    expect(text).toContain('5 star');
    expect(text).toContain('Free cancellation');
  });

  it('emits the selected room when reserve is clicked', () => {
    // Arrange
    spyOn(component.reserve, 'emit');
    const button: HTMLButtonElement = fixture.nativeElement.querySelector('button');

    // Act
    button.click();

    // Assert
    expect(component.reserve.emit).toHaveBeenCalledOnceWith(room);
  });
});
