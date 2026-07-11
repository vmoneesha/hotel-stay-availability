using System.Collections.Concurrent;
using HotelStay.Api.Domain.Dtos;
using HotelStay.Api.Domain.Normalization;
using HotelStay.Api.Domain.ProviderContracts;
using HotelStay.Api.Domain.Providers;
using HotelStay.Api.Domain.Services;
using HotelStay.Api.Validation;

namespace HotelStay.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddHotelStayServices(this IServiceCollection services)
    {
        services.AddSingleton<IHotelProvider, PremierStaysProvider>();
        services.AddSingleton<IHotelProvider, BudgetNestsProvider>();
        services.AddSingleton<IProviderRoomNormalizer, PremierStaysRoomNormalizer>();
        services.AddSingleton<IProviderRoomNormalizer, BudgetNestsRoomNormalizer>();
        services.AddSingleton<HotelSearchService>();
        services.AddSingleton<DocumentValidationService>();
        services.AddSingleton<ReservationService>();
        services.AddSingleton<HotelSearchCriteriaValidator>();
        services.AddSingleton<ReservationRequestValidator>();
        services.AddSingleton<ConcurrentDictionary<string, ReservationResponse>>();
        return services;
    }
}