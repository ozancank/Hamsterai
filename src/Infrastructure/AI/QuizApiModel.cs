using OCK.Core.Interfaces;

namespace Infrastructure.AI;

public sealed class QuizApiModel : IRequestModel
{
    public List<string?>? QuestionImages { get; set; }
    public List<string?>? QuestionTexts { get; set; }
    public List<bool>? VisualList { get; set; }
    public string? LessonName { get; set; }
    public long UserId { get; set; }
}