import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HotelSearchResponse, ReservationDto, ReserveRoomRequest, RoomType } from '../models/hotel.models';

@Injectable({ providedIn: 'root' })
export class HotelApiService {
  private readonly baseUrl = 'http://localhost:5000';

  constructor(private readonly http: HttpClient) {}

  search(destination: string, checkIn: string, checkOut: string, roomType: RoomType | ''): Observable<HotelSearchResponse> {
    let params = new HttpParams()
      .set('destination', destination)
      .set('checkIn', checkIn)
      .set('checkOut', checkOut);

    if (roomType) {
      params = params.set('roomType', roomType);
    }

    return this.http.get<HotelSearchResponse>(`${this.baseUrl}/hotels/search`, { params });
  }

  reserve(request: ReserveRoomRequest): Observable<ReservationDto> {
    return this.http.post<ReservationDto>(`${this.baseUrl}/hotels/reserve`, request);
  }

  getReservation(reference: string): Observable<ReservationDto> {
    return this.http.get<ReservationDto>(`${this.baseUrl}/hotels/reservation/${reference}`);
  }
}