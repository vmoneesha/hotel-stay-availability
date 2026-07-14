namespace HotelStay.Domain.Services;

public sealed class DeterministicReservationTimeProvider : TimeProvider
{
    private static readonly DateTimeOffset CreatedAtUtc = new(2026, 7, 11, 0, 0, 0, TimeSpan.Zero);

    public override DateTimeOffset GetUtcNow() => CreatedAtUtc;
}