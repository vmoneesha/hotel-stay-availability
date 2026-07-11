import { ComponentFixture, TestBed } from '@angular/core/testing';
import { SearchBarComponent } from './search-bar.component';
import { SearchFormValue } from '../../../models/hotel.models';

describe('SearchBarComponent', () => {
  let fixture: ComponentFixture<SearchBarComponent>;
  let component: SearchBarComponent;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SearchBarComponent]
    }).compileComponents();

    fixture = TestBed.createComponent(SearchBarComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('defaults check-in to today and check-out to tomorrow', () => {
    // Arrange
    const formValue = component.form.getRawValue();

    // Act
    const checkInInput: HTMLInputElement = fixture.nativeElement.querySelector('#checkIn');
    const checkOutInput: HTMLInputElement = fixture.nativeElement.querySelector('#checkOut');

    // Assert
    expect(formValue.checkIn).toBe(component.todayDate);
    expect(formValue.checkOut).toBe(component.minCheckOutDate());
    expect(checkInInput.min).toBe(component.todayDate);
    expect(checkOutInput.min).toBe(component.minCheckOutDate());
  });

  it('keeps the search button disabled until required fields are valid', () => {
    // Arrange
    const button = (): HTMLButtonElement => fixture.nativeElement.querySelector('button[type="submit"]');

    // Act
    fixture.detectChanges();

    // Assert
    expect(button().disabled).toBeTrue();

    // Act
    component.form.controls.destination.setValue('Hyderabad');
    fixture.detectChanges();

    // Assert
    expect(button().disabled).toBeFalse();
  });

  it('does not emit a search when required fields are missing', () => {
    // Arrange
    spyOn(component.search, 'emit');
    component.form.controls.destination.setValue('');

    // Act
    component.submit();

    // Assert
    expect(component.search.emit).not.toHaveBeenCalled();
    expect(component.form.invalid).toBeTrue();
  });

  it('normalizes a manually entered past check-in date back to today', () => {
    // Arrange
    const yesterday = addDays(component.todayDate, -1);
    component.form.controls.checkIn.setValue(yesterday);

    // Act
    component.normalizeDateInput('checkIn');

    // Assert
    expect(component.form.controls.checkIn.value).toBe(component.todayDate);
  });

  it('updates the check-out minimum and clears an invalid existing check-out date when check-in changes', () => {
    // Arrange
    const newCheckIn = addDays(component.todayDate, 2);
    const invalidCheckOut = addDays(component.todayDate, 2);
    component.form.controls.checkOut.setValue(invalidCheckOut);

    // Act
    component.form.controls.checkIn.setValue(newCheckIn);

    // Assert
    expect(component.minCheckOutDate()).toBe(addDays(newCheckIn, 1));
    expect(component.form.controls.checkOut.value).toBe('');
    expect(component.checkOutWasCleared).toBeTrue();
  });

  it('emits valid search criteria when the form is submitted', () => {
    // Arrange
    const expected: SearchFormValue = {
      destination: 'Mumbai',
      checkIn: component.todayDate,
      checkOut: component.minCheckOutDate(),
      roomType: 'Suite'
    };
    spyOn(component.search, 'emit');
    component.form.setValue(expected);

    // Act
    component.submit();

    // Assert
    expect(component.search.emit).toHaveBeenCalledOnceWith(expected);
  });

  function addDays(dateValue: string, days: number): string {
    const [year, month, day] = dateValue.split('-').map(Number);
    const date = new Date(year, month - 1, day + days);
    return `${date.getFullYear()}-${String(date.getMonth() + 1).padStart(2, '0')}-${String(date.getDate()).padStart(2, '0')}`;
  }
});
