import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { SelectedRoomContext } from '../models/hotel.models';

@Injectable({ providedIn: 'root' })
export class HotelSelectionStore {
  private readonly selectedRoomSubject = new BehaviorSubject<SelectedRoomContext | null>(null);
  readonly selectedRoom$ = this.selectedRoomSubject.asObservable();

  select(context: SelectedRoomContext): void {
    this.selectedRoomSubject.next(context);
  }

  current(): SelectedRoomContext | null {
    return this.selectedRoomSubject.value;
  }
}