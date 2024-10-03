
using Business.Services.QuestionService;
using Domain.Constants;

namespace WebAPI;

public class SenderHostedService(IServiceProvider serviceProvider) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(AppOptions.AITrySecond * 1000, stoppingToken);

                using var scope = serviceProvider.CreateScope();
                var questionService = scope.ServiceProvider.GetRequiredService<IQuestionService>();
                await questionService.SendForStatusSendAgain(stoppingToken);
            }
            catch { }
        }
    }
}
