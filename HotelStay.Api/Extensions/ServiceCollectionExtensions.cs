using HotelStay.Api.Mapping;
using HotelStay.Api.Providers;
using HotelStay.Api.Repositories;
using HotelStay.Api.Services;
using HotelStay.Api.Validation;

namespace HotelStay.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddHotelStayServices(this IServiceCollection services)
    {
        services.AddSingleton<IHotelProvider, PremierStaysProvider>();
        services.AddSingleton<IHotelProvider, BudgetNestsProvider>();
        services.AddSingleton<IProviderRoomMapper, PremierStaysRoomMapper>();
        services.AddSingleton<IProviderRoomMapper, BudgetNestsRoomMapper>();
        services.AddSingleton<IReservationRepository, InMemoryReservationRepository>();
        services.AddSingleton<IReservationReferenceGenerator, SequentialReservationReferenceGenerator>();
        services.AddScoped<IHotelSearchValidator, HotelSearchValidator>();
        services.AddScoped<IReservationValidator, ReservationValidator>();
        services.AddScoped<IHotelAvailabilityService, HotelAvailabilityService>();
        services.AddScoped<IReservationService, ReservationService>();
        return services;
    }
}