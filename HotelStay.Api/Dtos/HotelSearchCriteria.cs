namespace HotelStay.Api.Dtos;

public sealed record HotelSearchCriteria(
    string Destination,
    DateOnly CheckIn,
    DateOnly CheckOut,
    RoomType? RoomType)
{
    public int Nights => CheckOut.DayNumber - CheckIn.DayNumber;
}