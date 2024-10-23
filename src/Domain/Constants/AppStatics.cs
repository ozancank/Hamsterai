namespace Domain.Constants;

public class AppStatics
{
    public static bool SenderQuestionAllow { get; set; } = true;
    public static readonly SemaphoreSlim SenderSemaphore = new(AppOptions.SenderCapacity);
}