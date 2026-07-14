import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { HotelRoomDto, HotelSearchResponse, ReservationDto, ReserveRoomRequest, RoomType } from '../models/hotel.models';

@Injectable({ providedIn: 'root' })
export class HotelApiService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = environment.apiBaseUrl;

  search(destination: string, checkIn: string, checkOut: string, roomType: RoomType | ''): Observable<HotelSearchResponse> {
    let params = new HttpParams()
      .set('destination', destination)
      .set('checkIn', checkIn)
      .set('checkOut', checkOut);

    if (roomType) {
      params = params.set('roomType', roomType);
    }

    return this.http.get<HotelRoomDto[]>(`${this.baseUrl}/hotels/search`, { params })
      .pipe(map(rooms => ({ destination, checkIn, checkOut, rooms })));
  }

  reserve(request: ReserveRoomRequest): Observable<ReservationDto> {
    return this.http.post<ReservationDto>(`${this.baseUrl}/hotels/reserve`, request);
  }

  getReservation(reference: string): Observable<ReservationDto> {
    return this.http.get<ReservationDto>(`${this.baseUrl}/hotels/reservation/${reference}`);
  }
}