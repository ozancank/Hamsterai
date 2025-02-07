namespace Application.Features.Questions.Models.Questions;

public class UpdateQuestionTextRequestModel : IRequestModel
{
    public Guid QuestionId { get; set; }
    public string? QuestionText { get; set; }
}