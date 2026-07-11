import { Component, computed, signal } from '@angular/core';
import { Router } from '@angular/router';
import { finalize } from 'rxjs';
import { HotelRoomDto, SearchFormValue, SortOption, ValidationProblemResponse } from '../../models/hotel.models';
import { HotelApiService } from '../../services/hotel-api.service';
import { HotelSelectionStore } from '../../services/hotel-selection.store';
import { EmptyStateComponent } from '../../shared/components/empty-state/empty-state.component';
import { ErrorMessageComponent } from '../../shared/components/error-message/error-message.component';
import { HotelResultsListComponent } from '../../shared/components/hotel-results-list/hotel-results-list.component';
import { LoadingSpinnerComponent } from '../../shared/components/loading-spinner/loading-spinner.component';
import { SearchBarComponent } from '../../shared/components/search-bar/search-bar.component';

@Component({
  selector: 'app-search',
  imports: [EmptyStateComponent, ErrorMessageComponent, HotelResultsListComponent, LoadingSpinnerComponent, SearchBarComponent],
  templateUrl: './search.component.html',
  styleUrl: './search.component.scss'
})
export class SearchComponent {
  readonly rooms = signal<HotelRoomDto[]>([]);
  readonly validationErrors = signal<string[]>([]);
  readonly loading = signal(false);
  readonly searched = signal(false);
  readonly sortBy = signal<SortOption>('totalPrice');
  readonly lastSearch = signal<SearchFormValue | null>(null);
  readonly sortedRooms = computed(() => {
    const rooms = [...this.rooms()];
    return this.sortBy() === 'roomType'
      ? rooms.sort((left, right) => left.roomType.localeCompare(right.roomType) || left.totalStayPrice - right.totalStayPrice)
      : rooms.sort((left, right) => left.totalStayPrice - right.totalStayPrice || left.roomType.localeCompare(right.roomType));
  });

  constructor(
    private readonly hotelApi: HotelApiService,
    private readonly selectionStore: HotelSelectionStore,
    private readonly router: Router) {}

  search(criteria: SearchFormValue): void {
    this.lastSearch.set(criteria);
    this.validationErrors.set([]);
    this.searched.set(true);
    this.loading.set(true);
    this.hotelApi.search(criteria.destination, criteria.checkIn, criteria.checkOut, criteria.roomType)
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: response => {
          this.rooms.set(response.rooms);
        },
        error: error => {
          this.rooms.set([]);
          this.validationErrors.set(this.toErrorMessages(error.error));
        }
      });
  }

  reserve(room: HotelRoomDto): void {
    const criteria = this.lastSearch();
    if (!criteria) {
      this.validationErrors.set(['Search for stays before selecting a room.']);
      return;
    }

    this.selectionStore.select({ room, checkIn: criteria.checkIn, checkOut: criteria.checkOut });
    void this.router.navigate(['/reservation']);
  }

  updateSort(sortBy: SortOption): void {
    this.sortBy.set(sortBy);
  }

  private toErrorMessages(problem: ValidationProblemResponse | undefined): string[] {
    return problem?.details?.map(detail => detail.message) ?? ['Search failed.'];
  }
}