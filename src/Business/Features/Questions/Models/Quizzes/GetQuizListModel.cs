namespace Business.Features.Questions.Models.Quizzes;

public class GetQuizListModel : IResponseModel
{
    public string? Id { get; set; }
    public long UserId { get; set; }
    public byte LessonId { get; set; }
    public int TimeSecond { get; set; }
    public QuizStatus Status { get; set; }
    public byte CorrectCount { get; set; }
    public byte WrongCount { get; set; }
    public byte EmptyCount { get; set; }
    public double SuccessRate { get; set; }
    public byte QuestionCount { get; set; }
    public string? UserFullName { get; set; }
    public string? SchoolName { get; set; }
    public string? LessonName { get; set; }
    public List<string> GainNames { get; set; } = [];
}