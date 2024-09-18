using Infrastructure.AI;
using Infrastructure.AI.Seduss;
using Infrastructure.Time;
using Infrastructure.Time.WorldTimeApi;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class InfrastructureServiceRegistration
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        //services.AddScoped<ILicenceApi, LicenceApi>();
        services.AddScoped<ITimeApi, WorldTimeApi>();
        services.AddTransient<IQuestionApi, SedussApi>();

        return services;
    }
}