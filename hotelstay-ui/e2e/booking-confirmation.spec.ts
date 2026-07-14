import { test } from '@playwright/test';
import { BookingPage } from './helpers/booking-page';

test.describe('Booking confirmation', () => {
  test('shows reservation reference and booking summary after a successful booking', async ({ page }) => {
    const booking = new BookingPage(page);

    await booking.bookInternationalPassportStay('Mira Shah');

    await booking.expectConfirmationPageLoaded();
    await booking.expectConfirmationSummary({
      guestName: 'Mira Shah',
      provider: 'PremierStays',
      destination: 'London',
      roomType: 'Suite',
      cancellationPolicy: /Free cancellation|Non-refundable|Flexible|Refundable/i
    });
  });

  test('executes the booking flow in a desktop viewport', async ({ page }) => {
    await page.setViewportSize({ width: 1440, height: 900 });
    const booking = new BookingPage(page);

    await booking.bookDomesticNationalIdStay('Dev Patel');

    await booking.expectConfirmationPageLoaded();
    await booking.expectConfirmationSummary({
      guestName: 'Dev Patel',
      provider: 'BudgetNests',
      destination: 'Hyderabad',
      roomType: 'Standard',
      cancellationPolicy: /Free cancellation|Non-refundable|Flexible|Refundable/i
    });
  });

  test('executes the booking flow in a mobile viewport', async ({ page }) => {
    await page.setViewportSize({ width: 390, height: 844 });
    const booking = new BookingPage(page);

    await booking.bookDomesticNationalIdStay('Nina Iyer');

    await booking.expectConfirmationPageLoaded();
    await booking.expectConfirmationSummary({
      guestName: 'Nina Iyer',
      provider: 'BudgetNests',
      destination: 'Hyderabad',
      roomType: 'Standard',
      cancellationPolicy: /Free cancellation|Non-refundable|Flexible|Refundable/i
    });
  });
});