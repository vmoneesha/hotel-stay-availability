import { expect, test } from '@playwright/test';

test('searches for a room, reserves it, and shows confirmation', async ({ page }) => {
  await page.goto('/search');

  await searchForLondonSuite(page);

  await expect(page.getByRole('heading', { name: 'Premier London Regent' })).toBeVisible();
  await page.getByRole('button', { name: 'Reserve' }).click();

  await expect(page.getByRole('heading', { name: 'Finish your reservation' })).toBeVisible();
  await page.getByLabel('Guest name').fill('Asha Rao');
  await page.getByLabel('Document type').selectOption('Passport');
  await page.getByLabel('Document number').fill('P1234567');
  await page.getByRole('button', { name: 'Confirm reservation' }).click();

  await expect(page).toHaveURL(/\/confirmation\/HS-\d+/);
  await expect(page.getByRole('heading', { name: 'Your stay is confirmed' })).toBeVisible();
  await expect(page.getByText('Reservation reference')).toBeVisible();
  await expect(page.getByText('Asha Rao')).toBeVisible();
  await expect(page.getByText('PremierStays')).toBeVisible();
  await expect(page.getByText('London')).toBeVisible();
});

test('prevents searching when check-out is not after check-in', async ({ page }) => {
  await page.goto('/search');

  await page.getByLabel('Destination').selectOption('London');
  await page.getByLabel('Check-in').fill('2026-08-10');
  await page.getByLabel('Check-out').fill('2026-08-10');
  await page.getByLabel('Check-out').blur();

  await expect(page.getByRole('button', { name: /Search stays/i })).toBeDisabled();
  await expect(page.getByText('Select a new check-out date after the updated check-in date.')).toBeVisible();
});

test('blocks international reservation when National ID is selected', async ({ page }) => {
  await page.goto('/search');

  await searchForLondonSuite(page);
  await expect(page.getByRole('heading', { name: 'Premier London Regent' })).toBeVisible();
  await page.getByRole('button', { name: 'Reserve' }).click();

  await expect(page.getByRole('heading', { name: 'Finish your reservation' })).toBeVisible();
  await page.getByLabel('Guest name').fill('Asha Rao');
  await page.getByLabel('Document type').selectOption('NationalId');

  await expect(page.getByText('Passport needed for London')).toBeVisible();
  await expect(page.getByRole('button', { name: 'Select Passport to continue' })).toBeDisabled();
});

async function searchForLondonSuite(page: import('@playwright/test').Page): Promise<void> {
  await page.getByLabel('Destination').selectOption('London');
  await page.getByLabel('Check-in').fill('2026-08-10');
  await page.getByLabel('Check-out').fill('2026-08-13');
  await page.getByLabel('Room type').selectOption('Suite');
  await page.getByRole('button', { name: /Search stays/i }).click();
}