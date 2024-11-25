namespace Domain.Constants;

public class AppStatics
{
    public static readonly SemaphoreSlim QuestionSemaphore = new(AppOptions.QuestionSenderCapacity);
    public static readonly SemaphoreSlim SimilarSemaphore = new(AppOptions.SimilarSenderCapacity);
    public static readonly DateTime MilleniumDate = new(2000, 1, 1, 0, 0, 0);
    public static readonly QuestionStatus[] QuestionStatusesForCredit = [QuestionStatus.Waiting, QuestionStatus.Answered, QuestionStatus.SendAgain];
    public static bool SenderQuestionAllow { get; set; } = true;
    public static bool SenderSimilarAllow { get; set; } = true;
    public static Dictionary<string, Dictionary<string, int>> Enums { get; set; } = [];
    public static Dictionary<string, List<PropertyDto>> Entities { get; set; } = [];

    public record PropertyDto(string Name, string Type, bool IsNullable, bool IsReadOnly, bool İsVirtual);
}