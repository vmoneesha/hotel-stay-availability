import { expect, test } from '@playwright/test';
import { BookingPage } from './helpers/booking-page';

test.describe('Validation', () => {
  test('prevents selecting a past check-in date', async ({ page }) => {
    const booking = new BookingPage(page);

    await booking.gotoSearch();
    await page.getByLabel('Destination').selectOption('Hyderabad');
    await page.getByLabel('Check-in').fill('2026-07-13');
    await page.getByLabel('Check-in').blur();

    await expect(page.getByLabel('Check-in')).toHaveValue('2026-07-14');
  });

  test('prevents selecting a check-out date before the check-in date', async ({ page }) => {
    const booking = new BookingPage(page);

    await booking.gotoSearch();
    await page.getByLabel('Destination').selectOption('London');
    await page.getByLabel('Check-in').fill('2026-08-10');
    await page.getByLabel('Check-out').fill('2026-08-09');
    await page.getByLabel('Check-out').blur();

    await expect(page.getByRole('button', { name: /Search stays/i })).toBeDisabled();
    await expect(page.getByText('Select a new check-out date after the updated check-in date.')).toBeVisible();
  });

  test('shows required field validation on the reservation form', async ({ page }) => {
    const booking = new BookingPage(page);

    await booking.gotoSearch();
    await booking.searchForHyderabad('Standard');
    await booking.selectFirstRoom();
    await page.getByLabel('Guest name').focus();
    await page.getByLabel('Guest name').blur();
    await page.getByLabel('Document number').focus();
    await page.getByLabel('Document number').blur();

    await expect(page.getByText('Guest name is required.')).toBeVisible();
    await expect(page.getByText('Document number is required.')).toBeVisible();
    await expect(page.getByRole('button', { name: 'Confirm reservation' })).toBeDisabled();
  });

  test('shows an invalid document selection message', async ({ page }) => {
    const booking = new BookingPage(page);

    await booking.gotoSearch();
    await booking.searchForLondon('Suite');
    await booking.selectFirstRoom();
    await page.getByLabel('Document type').selectOption('NationalId');

    await expect(page.getByText('Passport needed for London')).toBeVisible();
    await expect(page.getByText('London requires Passport for confirmed reservations. You can review National ID, but switch to Passport before continuing.')).toBeVisible();
  });

  test('displays HTTP 422 validation errors returned by the API', async ({ page }) => {
    await page.route('**/hotels/reserve', route => route.fulfill({
      status: 422,
      contentType: 'application/json',
      body: JSON.stringify({
        title: 'Document validation failed.',
        details: [
          { field: 'DocumentType', message: 'Passport is required for international destinations.' }
        ]
      })
    }));
    const booking = new BookingPage(page);

    await booking.gotoSearch();
    await booking.searchForHyderabad('Standard');
    await booking.selectFirstRoom();
    await booking.completeReservation({
      guestName: 'Asha Rao',
      documentType: 'NationalId',
      documentNumber: 'N1234567'
    });

    await expect(page.getByText('Passport is required for international destinations.')).toBeVisible();
  });
});