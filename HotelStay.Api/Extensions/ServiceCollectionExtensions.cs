using HotelStay.Domain.Normalization;
using HotelStay.Domain.ProviderContracts;
using HotelStay.Domain.Providers;
using HotelStay.Domain.Services;
using HotelStay.Domain.Stores;
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
        services.AddSingleton<IReservationStore, InMemoryReservationStore>();
        services.AddSingleton<HotelSearchCriteriaValidator>();
        services.AddSingleton<ReservationRequestValidator>();
        services.AddSingleton(TimeProvider.System);
        return services;
    }
}