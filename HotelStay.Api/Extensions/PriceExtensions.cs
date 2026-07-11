namespace HotelStay.Api.Extensions;

public static class PriceExtensions
{
    public static decimal TotalForStay(this decimal perNightPrice, int nights) => decimal.Round(perNightPrice * nights, 2);
}