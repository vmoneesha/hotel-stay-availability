import { expect, test } from '@playwright/test';
import { BookingPage } from './helpers/booking-page';

test.describe('Reservation', () => {
  test('completes a domestic reservation with National ID', async ({ page }) => {
    const booking = new BookingPage(page);

    await booking.bookDomesticNationalIdStay('Asha Rao');

    await booking.expectConfirmationPageLoaded();
    await booking.expectConfirmationSummary({
      guestName: 'Asha Rao',
      provider: 'BudgetNests',
      destination: 'Hyderabad',
      roomType: 'Standard',
      cancellationPolicy: /Free cancellation|Non-refundable|Flexible|Refundable/i
    });
  });

  test('completes an international reservation with Passport', async ({ page }) => {
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

  test('shows document rules before allowing an international reservation', async ({ page }) => {
    const booking = new BookingPage(page);

    await booking.gotoSearch();
    await booking.searchForLondon('Suite');
    await booking.selectFirstRoom();
    await page.getByLabel('Document type').selectOption('NationalId');

    await expect(page.getByText('Passport needed for London')).toBeVisible();
    await expect(page.getByText('National ID is available to select, but it cannot be used to complete an international booking.')).toBeVisible();
    await expect(page.getByRole('button', { name: 'Select Passport to continue' })).toBeDisabled();
  });
});