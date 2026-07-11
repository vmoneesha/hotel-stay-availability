import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { HotelSearchResponse, ReservationDto, ReserveRoomRequest, RoomType } from '../models/hotel.models';

@Injectable({ providedIn: 'root' })
export class HotelApiService {
  private readonly baseUrl = environment.apiBaseUrl;

  constructor(private readonly http: HttpClient) {}

  search(destination: string, checkIn: string, checkOut: string, roomType: RoomType | ''): Observable<HotelSearchResponse> {
    let params = new HttpParams()
      .set('destination', destination)
      .set('checkIn', checkIn)
      .set('checkOut', checkOut);

    if (roomType) {
      params = params.set('roomType', roomType);
    }

    return this.http.get<HotelSearchResponse | HotelSearchResponse['rooms']>(`${this.baseUrl}/hotels/search`, { params })
      .pipe(map(response => Array.isArray(response)
        ? { destination, checkIn, checkOut, rooms: response }
        : response));
  }

  reserve(request: ReserveRoomRequest): Observable<ReservationDto> {
    return this.http.post<ReservationDto>(`${this.baseUrl}/hotels/reserve`, request);
  }

  getReservation(reference: string): Observable<ReservationDto> {
    return this.http.get<ReservationDto>(`${this.baseUrl}/hotels/reservation/${reference}`);
  }
}