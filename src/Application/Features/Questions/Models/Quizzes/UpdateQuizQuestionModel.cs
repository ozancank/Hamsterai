namespace Application.Features.Questions.Models.Quizzes;

public class UpdateQuizQuestionModel : IRequestModel
{
    public string? QuestionId { get; set; }
    public char? AnswerOption { get; set; }
}