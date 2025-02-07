namespace Application.Features.Questions.Models.Quizzes;

public class AddQuizModel : IRequestModel
{
    public short LessonId { get; set; }
    public string? LessonName { get; set; }
    public long UserId { get; set; }
    public List<Similar> QuestionList { get; set; } = [];
}