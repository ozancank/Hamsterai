namespace Domain.Constants;

public class AppStatics
{
    public static readonly SemaphoreSlim SenderSemaphore = new(AppOptions.SenderCapacity);
    public static bool SenderQuestionAllow { get; set; } = true;
    public static Dictionary<string, Dictionary<string, int>> Enums { get; set; } = [];
    public static Dictionary<string, List<PropertyDto>> Entities { get; set; } = [];

    public record PropertyDto(string Name, string Type, bool IsNullable, bool IsReadOnly, bool isVirtual);
}