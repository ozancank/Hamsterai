using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class InfrastructureServiceRegistration
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        //services.AddScoped<ILicenceApi, LicenceApi>();
        services.AddScoped<Time.ITimeApi, Time.WorldTimeApi.WorldTimeApi>();
        services.AddTransient<AI.IQuestionApi, AI.Seduss.SedussApi>();
        services.AddSingleton<Notification.INotificationApi, Notification.Firebase.FirebaseApi>();
        services.AddTransient<OCR.IOcrApi, OCR.Gemini.GeminiApi>();

        return services;
    }
}