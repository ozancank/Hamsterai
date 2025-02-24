namespace Application.Features.Questions.Models.Questions;

public class AddQuestionTextModel : IRequestModel
{
    public Guid QuestionId { get; set; } = Guid.Empty;
    public short LessonId { get; set; }
    public string? QuestionText { get; set; }
    public QuestionType Type { get; set; } = QuestionType.Question;
}