export type RoomType = 'Standard' | 'Deluxe' | 'Suite';
export type DocumentType = 'NationalId' | 'Passport';
export type SortOption = 'totalPrice' | 'roomType';

export interface HotelRoomDto {
  providerCode: string;
  providerBadge: string;
  hotelId: string;
  hotelName: string;
  destination: string;
  roomId: string;
  roomType: RoomType;
  perNightPrice: number;
  totalStayPrice: number;
  nights: number;
  amenities: string[];
  starRating: number | null;
  cancellationPolicy: string;
  cancellationPolicyDescription: string;
}

export interface HotelSearchResponse {
  destination: string;
  checkIn: string;
  checkOut: string;
  rooms: HotelRoomDto[];
}

export interface ReserveRoomRequest {
  destination: string;
  checkIn: string;
  checkOut: string;
  providerCode: string;
  hotelId: string;
  roomId: string;
  roomType: RoomType;
  guestName: string;
  documentType: DocumentType;
  documentNumber: string;
  perNightPrice: number;
}

export interface ReservationDto extends ReserveRoomRequest {
  reference: string;
  totalStayPrice: number;
  nights: number;
  createdAtUtc: string;
}

export interface ValidationProblemResponse {
  error: string;
  details: Array<{ field: string; message: string }>;
}

export interface SelectedRoomContext {
  room: HotelRoomDto;
  checkIn: string;
  checkOut: string;
}

export interface SearchFormValue {
  destination: string;
  checkIn: string;
  checkOut: string;
  roomType: RoomType | '';
}