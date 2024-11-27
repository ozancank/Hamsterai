using Application.Services.QuestionService;
using Domain.Constants;
using OCK.Core.Logging.Serilog;

namespace WebAPI.HostedServices;

public class QuestionHostedService(IServiceProvider serviceProvider, LoggerServiceBase loggerServiceBase) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(AppOptions.AIQuestionSendSecond * 1000, stoppingToken);

                if (!AppStatics.SenderQuestionAllow) continue;

                using var scope = serviceProvider.CreateScope();
                var questionService = scope.ServiceProvider.GetRequiredService<IQuestionService>();
                await questionService.SendQuestions(stoppingToken);
            }
            catch (Exception ex)
            {
                loggerServiceBase.Error($"{nameof(SimilarHostedService)} {DateTime.Now:yyyy-MM-dd HH:mm:ss}*{ex?.InnerException?.Message}*{ex?.Message}*{ex?.StackTrace}");
            }
        }
    }
}