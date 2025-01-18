using Domain.Constants;
using OCK.Core.Interfaces;

namespace Infrastructure.AI;

public sealed class QuestionApiModel : IRequestModel
{
    public Guid Id { get; set; }
    public string? Base64 { get; set; }
    public short LessonId { get; set; }
    public string? LessonName { get; set; }
    public long UserId { get; set; }
    public string? QuestionText { get; set; }
    public bool ExcludeQuiz { get; set; }
    public string? AIUrl { get; set; }
    public QuestionType QuestionType { get; set; } = QuestionType.Question;
}