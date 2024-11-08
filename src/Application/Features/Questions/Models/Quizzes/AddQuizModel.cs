namespace Application.Features.Questions.Models.Quizzes;

public class AddQuizModel : IRequestModel
{
    public short LessonId { get; set; }
    public long UserId { get; set; }
    public List<string> QuestionList { get; set; } = [];
    public List<bool> VisualList { get; set; } = [];
}