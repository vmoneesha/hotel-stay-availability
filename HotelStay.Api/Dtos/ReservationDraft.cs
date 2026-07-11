namespace HotelStay.Api.Dtos;

public sealed record ReservationDraft(
    string Destination,
    DateOnly CheckIn,
    DateOnly CheckOut,
    string Provider,
    string HotelId,
    string RoomId,
    RoomType RoomType,
    string GuestName,
    DocumentType DocumentType,
    string DocumentNumber,
    decimal PerNightPrice)
{
    public int Nights => CheckOut.DayNumber - CheckIn.DayNumber;
    public decimal TotalStayPrice => PerNightPrice * Nights;
}