namespace HotelStay.Api.Dtos;

public sealed record HotelSearchResponse(
    string Destination,
    DateOnly CheckIn,
    DateOnly CheckOut,
    IReadOnlyCollection<HotelRoomDto> Rooms);