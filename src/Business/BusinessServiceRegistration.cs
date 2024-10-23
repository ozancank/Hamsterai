using AutoMapper.EquivalencyExpression;
using DataAccess;
using Infrastructure;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Business;

public static class BusinessServiceRegistration
{
    public static IServiceCollection AddBusinessServices(this IServiceCollection services)
    {
        services.AddAutoMapper(x =>
        {
            x.AddCollectionMappers();
        }, Assembly.GetExecutingAssembly());
        services.AddSubClassesOfType(Assembly.GetExecutingAssembly(), typeof(IBusinessRule));
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(Behaviors.AuthorizationBehavior<,>));
        //services.AddTransient(typeof(IPipelineBehavior<,>), typeof(OCK.Core.Pipelines.Caching.CachingBehavior<,>));
        //services.AddTransient(typeof(IPipelineBehavior<,>), typeof(OCK.Core.Pipelines.Caching.CacheRemovingBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(OCK.Core.Pipelines.Logging.LoggingBehavior<,>));
        //services.AddTransient(typeof(IPipelineBehavior<,>), typeof(OCK.Core.Pipelines.Performance.PerformanceBehavior<,>));
        //services.AddTransient(typeof(IPipelineBehavior<,>), typeof(OCK.Core.Pipelines.Transaction.TransactionScopeBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(OCK.Core.Pipelines.Validation.RequestValidationBehavior<,>));

        services.AddSingleton<OCK.Core.Logging.Serilog.LoggerServiceBase, OCK.Core.Logging.Serilog.Logger.PostgreSqlLogger>();
        services.AddSingleton<OCK.Core.Mailing.IMailService, OCK.Core.Mailing.MailKitImplementations.MailKitMailService>();

        services.AddScoped<Services.AuthService.IAuthService, Services.AuthService.AuthManager>();
        services.AddScoped<Services.CommonService.ICommonService, Services.CommonService.CommonManager>();
        services.AddScoped<Services.EmailService.IEmailService, Services.EmailService.EmailManager>();
        services.AddScoped<Services.GainService.IGainService, Services.GainService.GainManager>();
        services.AddScoped<Services.NotificationService.INotificationService, Services.NotificationService.NotificationManager>();
        services.AddScoped<Services.QuestionService.IQuestionService, Services.QuestionService.QuestionManagerWithoutOcr>();
        services.AddScoped<Services.UserService.IUserService, Services.UserService.UserManager>();

        services.AddDALServices();
        services.AddInfrastructureServices();
        return services;
    }
}