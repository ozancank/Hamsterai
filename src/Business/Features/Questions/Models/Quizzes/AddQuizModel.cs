namespace Business.Features.Questions.Models.Quizzes;

public class AddQuizModel : IRequestModel
{
    public byte LessonId { get; set; }
    public long UserId { get; set; }
    public List<string> Base64List { get; set; }
    public List<string> QuestionList { get; set; }
}