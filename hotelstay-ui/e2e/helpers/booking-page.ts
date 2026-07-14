import { expect, type Locator, type Page } from '@playwright/test';

export type RoomType = 'Standard' | 'Deluxe' | 'Suite' | '';
export type DocumentType = 'NationalId' | 'Passport';

export interface SearchCriteria {
  destination: string;
  checkIn: string;
  checkOut: string;
  roomType?: RoomType;
}

export interface GuestDetails {
  guestName: string;
  documentType: DocumentType;
  documentNumber: string;
}

export class BookingPage {
  constructor(private readonly page: Page) {}

  readonly resultCards = this.page.locator('app-hotel-card');
  readonly reservationReference = this.page.locator('.reference-pill strong');

  async gotoSearch(): Promise<void> {
    await this.page.goto('/search');
    await this.expectSearchPageLoaded();
  }

  async expectSearchPageLoaded(): Promise<void> {
    await expect(this.page.getByRole('heading', { name: /Find a room/i })).toBeVisible();
    await expect(this.page.getByRole('button', { name: /Search stays/i })).toBeVisible();
  }

  async search(criteria: SearchCriteria): Promise<void> {
    await this.page.getByLabel('Destination').selectOption(criteria.destination);
    await this.page.getByLabel('Check-in').fill(criteria.checkIn);
    await this.page.getByLabel('Check-out').fill(criteria.checkOut);

    if (criteria.roomType !== undefined) {
      await this.page.getByLabel('Room type').selectOption(criteria.roomType);
    }

    await this.page.getByRole('button', { name: /Search stays/i }).click();
  }

  async searchForHyderabad(roomType: RoomType = ''): Promise<void> {
    await this.search({
      destination: 'Hyderabad',
      checkIn: '2026-08-10',
      checkOut: '2026-08-12',
      roomType
    });
  }

  async searchForLondon(roomType: RoomType = 'Suite'): Promise<void> {
    await this.search({
      destination: 'London',
      checkIn: '2026-08-10',
      checkOut: '2026-08-13',
      roomType
    });
  }

  async expectResultsDisplayed(): Promise<void> {
    await expect(this.page.getByRole('heading', { name: /Premier|Budget/ }).first()).toBeVisible();
    await expect(this.resultCards.first()).toBeVisible();
  }

  async expectProviderBadges(...providerNames: string[]): Promise<void> {
    for (const providerName of providerNames) {
      await expect(this.page.getByText(providerName).first()).toBeVisible();
    }
  }

  async expectRoomDetailsVisible(): Promise<void> {
    const firstCard = this.resultCards.first();
    await expect(firstCard.getByRole('heading')).toBeVisible();
    await expect(firstCard.getByLabel('Room price')).toBeVisible();
    await expect(firstCard.getByRole('button', { name: 'Reserve' })).toBeVisible();
  }

  async sortBy(sortOption: 'totalPrice' | 'roomType'): Promise<void> {
    await this.page.getByLabel('Sort by').selectOption(sortOption);
  }

  async expectResultsSortedByTotalPrice(): Promise<void> {
    const prices = await this.totalPrices();
    expect(prices.length).toBeGreaterThan(1);
    expect(prices).toEqual([...prices].sort((left, right) => left - right));
  }

  async selectFirstRoom(): Promise<void> {
    await this.resultCards.first().getByRole('button', { name: 'Reserve' }).click();
    await this.expectReservationPageLoaded();
  }

  async expectReservationPageLoaded(): Promise<void> {
    await expect(this.page.getByRole('heading', { name: 'Finish your reservation' })).toBeVisible();
  }

  async completeReservation(details: GuestDetails): Promise<void> {
    await this.page.getByLabel('Guest name').fill(details.guestName);
    await this.page.getByLabel('Document type').selectOption(details.documentType);
    await this.page.getByLabel('Document number').fill(details.documentNumber);
    await this.page.getByRole('button', { name: 'Confirm reservation' }).click();
  }

  async expectConfirmationPageLoaded(): Promise<void> {
    await expect(this.page).toHaveURL(/\/confirmation\/HS-\d+/);
    await expect(this.page.getByRole('heading', { name: 'Your stay is confirmed' })).toBeVisible();
  }

  async expectConfirmationSummary(details: {
    guestName: string;
    provider: string;
    destination: string;
    roomType: string;
    cancellationPolicy: RegExp;
  }): Promise<void> {
    await expect(this.reservationReference).toHaveText(/HS-\d{6}/);
    await expect(this.page.getByText(details.guestName)).toBeVisible();
    await expect(this.detailValue('Provider')).toContainText(details.provider);
    await expect(this.detailValue('Destination')).toContainText(details.destination);
    await expect(this.detailValue('Room type')).toContainText(details.roomType);
    await expect(this.detailValue('Total price')).toContainText(/₹|INR|,/);
    await expect(this.detailValue('Cancellation policy')).toContainText(details.cancellationPolicy);
  }

  async bookDomesticNationalIdStay(guestName = 'Asha Rao'): Promise<void> {
    await this.gotoSearch();
    await this.searchForHyderabad('Standard');
    await this.expectResultsDisplayed();
    await this.selectFirstRoom();
    await this.completeReservation({
      guestName,
      documentType: 'NationalId',
      documentNumber: 'N1234567'
    });
  }

  async bookInternationalPassportStay(guestName = 'Mira Shah'): Promise<void> {
    await this.gotoSearch();
    await this.searchForLondon('Suite');
    await this.expectResultsDisplayed();
    await this.selectFirstRoom();
    await this.completeReservation({
      guestName,
      documentType: 'Passport',
      documentNumber: 'P1234567'
    });
  }

  private async totalPrices(): Promise<number[]> {
    const priceTexts = await this.resultCards.locator('app-price-card strong').allTextContents();
    return priceTexts.map(price => Number(price.replace(/[^0-9.]/g, '')));
  }

  private detailValue(label: string): Locator {
    return this.page.locator('dl.confirmation-details div').filter({ has: this.page.locator('dt', { hasText: label }) }).locator('dd');
  }
}