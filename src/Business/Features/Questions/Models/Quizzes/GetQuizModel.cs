namespace Business.Features.Questions.Models.Quizzes;

public class GetQuizModel : IResponseModel
{
    public string Id { get; set; }
    public long UserId { get; set; }
    public byte LessonId { get; set; }
    public int TimeSecond { get; set; }
    public QuizStatus Status { get; set; }
    public byte CorrectCount { get; set; }
    public byte WrongCount { get; set; }
    public byte EmptyCount { get; set; }
    public double SuccessRate { get; set; }
    public List<string> GainNames { get; set; }
    public List<GetQuizQuestionModel> Questions { get; set; }
    public Dictionary<byte, char> RightOptions { get; set; }
    public Dictionary<byte, char?> Answers { get; set; }

    public string UserFullName { get; set; }
    public string SchoolName { get; set; }
    public string LessonName { get; set; }

    public GetQuizModel()
    {
        GainNames = [];
        Questions = [];
        RightOptions = [];
        Answers = [];
    }
}