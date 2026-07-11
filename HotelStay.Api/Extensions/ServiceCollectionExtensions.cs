using System.Collections.Concurrent;
using HotelStay.Api.Domain.Dtos;
using HotelStay.Api.Domain.ProviderContracts;
using HotelStay.Api.Domain.Providers;
using HotelStay.Api.Domain.Services;

namespace HotelStay.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddHotelStayServices(this IServiceCollection services)
    {
        services.AddSingleton<IHotelProvider, PremierStaysProvider>();
        services.AddSingleton<IHotelProvider, BudgetNestsProvider>();
        services.AddSingleton<HotelSearchService>();
        services.AddSingleton<DocumentValidationService>();
        services.AddSingleton<ReservationService>();
        services.AddSingleton<ConcurrentDictionary<string, ReservationResponse>>();
        return services;
    }
}