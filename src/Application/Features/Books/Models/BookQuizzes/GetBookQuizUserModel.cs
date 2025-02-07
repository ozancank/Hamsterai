using Application.Features.Books.Dto;

namespace Application.Features.Books.Models.BookQuizzes;

public class GetBookQuizUserModel : IResponseModel
{
    public Guid Id { get; set; }
    public long UserId { get; set; }
    public int BookId { get; set; }
    public string? BookName { get; set; }
    public short LessonId { get; set; }
    public string? LessonName { get; set; }
    public string? QuizName { get; set; }
    public byte QuestionCount { get; set; }
    public byte OptionCount { get; set; }
    public OptionDto[] UserAnswers { get; set; } = [];
}