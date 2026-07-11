import { Component, input, output } from '@angular/core';
import { HotelRoomDto, SortOption } from '../../../models/hotel.models';
import { HotelCardComponent } from '../hotel-card/hotel-card.component';

@Component({
  selector: 'app-hotel-results-list',
  imports: [HotelCardComponent],
  templateUrl: './hotel-results-list.component.html',
  styleUrl: './hotel-results-list.component.scss'
})
export class HotelResultsListComponent {
  readonly rooms = input<readonly HotelRoomDto[]>([]);
  readonly sortBy = input.required<SortOption>();
  readonly sortChange = output<SortOption>();
  readonly reserve = output<HotelRoomDto>();
}
