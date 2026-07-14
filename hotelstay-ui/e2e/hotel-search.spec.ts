import { test } from '@playwright/test';
import { BookingPage } from './helpers/booking-page';

test.describe('Hotel search', () => {
  test('loads the search page and displays matching hotel rooms', async ({ page }) => {
    const booking = new BookingPage(page);

    await booking.gotoSearch();
    await booking.searchForHyderabad();

    await booking.expectResultsDisplayed();
    await booking.expectProviderBadges('Premier', 'Budget');
    await booking.expectRoomDetailsVisible();
  });

  test('sorts search results by total price', async ({ page }) => {
    const booking = new BookingPage(page);

    await booking.gotoSearch();
    await booking.searchForHyderabad();
    await booking.expectResultsDisplayed();

    await booking.sortBy('roomType');
    await booking.sortBy('totalPrice');

    await booking.expectResultsSortedByTotalPrice();
  });
});