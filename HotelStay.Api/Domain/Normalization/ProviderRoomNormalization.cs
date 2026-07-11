using HotelStay.Api.Domain.Dtos;
using HotelStay.Api.Domain.Enums;

namespace HotelStay.Api.Domain.Normalization;

internal static class ProviderRoomNormalization
{
    public static RoomType ParseRoomType(string value) => Enum.Parse<RoomType>(value, ignoreCase: true);

    public static CancellationPolicy ParseCancellationPolicy(string value)
    {
        if (value.Contains("non_refundable", StringComparison.OrdinalIgnoreCase) ||
            value.Contains("non-refundable", StringComparison.OrdinalIgnoreCase))
        {
            return CancellationPolicy.NonRefundable;
        }

        if (value.Contains("free", StringComparison.OrdinalIgnoreCase))
        {
            return CancellationPolicy.FreeCancellation;
        }

        if (value.Contains("refund", StringComparison.OrdinalIgnoreCase))
        {
            return CancellationPolicy.Refundable;
        }

        return CancellationPolicy.Flexible;
    }

    public static string NormalizeCancellationDescription(string value) =>
        ParseCancellationPolicy(value) switch
        {
            CancellationPolicy.NonRefundable => "Non-refundable",
            CancellationPolicy.FreeCancellation => "Free cancellation",
            CancellationPolicy.Refundable => "Refundable",
            _ => "Flexible cancellation"
        };

    public static decimal CalculateTotalStayPrice(decimal perNightPrice, HotelSearchRequest request) =>
        decimal.Round(perNightPrice * CalculateNights(request), 2);

    public static int CalculateNights(HotelSearchRequest request) => request.CheckOut.DayNumber - request.CheckIn.DayNumber;
}