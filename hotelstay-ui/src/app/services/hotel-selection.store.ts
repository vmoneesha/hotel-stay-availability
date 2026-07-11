import { Injectable, signal } from '@angular/core';
import { HotelRoomDto, SelectedRoomContext } from '../models/hotel.models';

@Injectable({ providedIn: 'root' })
export class HotelSelectionStore {
  private readonly selectedRoomSignal = signal<SelectedRoomContext | null>(null);
  private readonly confirmationRooms = new Map<string, HotelRoomDto>();

  readonly selectedRoom = this.selectedRoomSignal.asReadonly();

  select(context: SelectedRoomContext): void {
    this.selectedRoomSignal.set(context);
  }

  current(): SelectedRoomContext | null {
    return this.selectedRoomSignal();
  }

  rememberConfirmation(reference: string, room: HotelRoomDto): void {
    this.confirmationRooms.set(reference, room);
  }

  confirmedRoom(reference: string): HotelRoomDto | null {
    return this.confirmationRooms.get(reference) ?? null;
  }
}