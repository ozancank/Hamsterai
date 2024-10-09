using Business.Services.QuestionService;
using Domain.Constants;

namespace WebAPI;

public class QuizHostedService(IServiceProvider serviceProvider) : BackgroundService
{
    public override Task StartAsync(CancellationToken cancellationToken)
    {
        return base.StartAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var currentTime = DateTime.Now;

                if (currentTime.Hour >= 1 && currentTime.Hour < 7)
                {
                    using var scope = serviceProvider.CreateScope();
                    var questionService = scope.ServiceProvider.GetRequiredService<IQuestionService>();
                    await questionService.AddQuiz(false, stoppingToken);
                }

                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
            catch { }            
        }
    }
}