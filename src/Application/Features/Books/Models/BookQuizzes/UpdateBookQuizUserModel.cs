using Application.Features.Books.Dto;

namespace Application.Features.Books.Models.BookQuizzes;

public class UpdateBookQuizUserModel : IRequestModel
{
    public Guid BookQuizId { get; set; }
    public OptionDto[] UserAnswers { get; set; } = [];
    public QuizStatus Status { get; set; }
}