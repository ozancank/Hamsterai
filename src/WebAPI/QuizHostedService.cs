namespace WebAPI;

public class QuizHostedService/*(IServiceProvider serviceProvider)*/ : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var now = DateTime.Now;
            var scheduledTime = DateTime.Today.AddHours(2);
            if (now > scheduledTime) scheduledTime = scheduledTime.AddDays(1);
            var delay = scheduledTime - now;
            await Task.Delay(delay, stoppingToken);

            // çalışacak kod buraya yazılır
            //using var scope = serviceProvider.CreateScope();
            //var questionService = scope.ServiceProvider.GetRequiredService<IQuestionService>();
            //await questionService.AddQuiz(stoppingToken);
            //_ = CheckAndSendEmails();

            await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
        }
    }
}