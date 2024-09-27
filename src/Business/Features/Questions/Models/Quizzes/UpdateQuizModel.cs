namespace Business.Features.Questions.Models.Quizzes;

public class UpdateQuizModel : IRequestModel
{
    public string QuizId { get; set; }
    public int TimeSecond { get; set; }
    public QuizStatus Status { get; set; }
    public List<UpdateQuizQuestionModel> Answers { get; set; }
}
