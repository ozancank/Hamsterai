namespace Domain.Constants;

public class AppStatics
{
    public static readonly SemaphoreSlim SenderSemaphore = new(AppOptions.SenderCapacity);
    public static bool SenderQuestionAllow { get; set; } = true;
    public static Dictionary<string, Dictionary<string, int>> Enums { get; set; } = [];
}