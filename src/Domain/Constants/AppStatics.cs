namespace Domain.Constants;

public class AppStatics
{
    public static readonly SemaphoreSlim QuestionSemaphore = new(AppOptions.AIQuestionSenderCapacity);
    public static readonly SemaphoreSlim SimilarSemaphore = new(AppOptions.AISimilarSenderCapacity);
    public static readonly SemaphoreSlim GainSemaphore = new(AppOptions.AIGainSenderCapacity);
    public static readonly DateTime MilleniumDate = new(2000, 1, 1, 0, 0, 0);
    public static readonly QuestionStatus[] QuestionStatusesForGain = [QuestionStatus.Waiting, QuestionStatus.Answered, QuestionStatus.SendAgain];
    public static readonly QuestionStatus[] QuestionStatusesForCredit = [QuestionStatus.Waiting, QuestionStatus.Answered, QuestionStatus.SendAgain];
    public static readonly QuestionStatus[] QuestionStatusesForAdmin = [QuestionStatus.Waiting, QuestionStatus.Answered, QuestionStatus.SendAgain];
    public static readonly QuestionStatus[] QuestionStatusesForSender = [QuestionStatus.Waiting, QuestionStatus.Error, QuestionStatus.SendAgain, QuestionStatus.ConnectionError, QuestionStatus.Timeout];

    public static readonly char[] OptionChars = ['A', 'B', 'C', 'D', 'E'];
    public static readonly string[] OptionStrings = ["A", "B", "C", "D", "E"];
    public static readonly string[] OptionsWithParentheses = ["A) ", "B) ", "C) ", "D) ", "E) "];

    public static bool SenderQuestionAllow { get; set; } = true;
    public static bool SenderSimilarAllow { get; set; } = true;
    public static bool SenderGainAllow { get; set; } = true;
    public static Dictionary<string, Dictionary<string, int>> Enums { get; set; } = [];
    public static Dictionary<string, List<PropertyDto>> Entities { get; set; } = [];

    public record PropertyDto(string Name, string Type, bool IsNullable, bool IsReadOnly, bool İsVirtual);
}