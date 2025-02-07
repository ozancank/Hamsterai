using Application.Features.Books.Dto;

namespace Application.Features.Books.Models.BookQuizzes;

public class GetBookQuizModel : IResponseModel
{
    public Guid Id { get; set; }
    public bool IsActive { get; set; }
    public long CreateUser { get; set; }
    public DateTime CreateDate { get; set; }
    public long UpdateUser { get; set; }
    public DateTime UpdateDate { get; set; }
    public int BookId { get; set; }
    public string? BookName { get; set; }
    public short LessonId { get; set; }
    public string? LessonName { get; set; }
    public string? Name { get; set; }
    public byte QuestionCount { get; set; }
    public byte OptionCount { get; set; }
    public OptionDto[] RightAnswers { get; set; } = [];
}