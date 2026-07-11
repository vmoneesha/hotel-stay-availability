import { Component, input } from '@angular/core';
import { RoomType } from '../../../models/hotel.models';

@Component({
  selector: 'app-room-type-badge',
  template: `<span class="room-type-badge">{{ roomType() }}</span>`,
  styleUrl: './room-type-badge.component.scss'
})
export class RoomTypeBadgeComponent {
  readonly roomType = input.required<RoomType>();
}
