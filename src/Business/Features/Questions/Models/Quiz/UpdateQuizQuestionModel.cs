namespace Business.Features.Questions.Models.Quiz;

public class UpdateQuizQuestionModel : IRequestModel
{
    public string QuestionId { get; set; }
    public char? AnswerOption { get; set; }
}