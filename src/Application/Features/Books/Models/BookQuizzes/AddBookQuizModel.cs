using Application.Features.Books.Dto;

namespace Application.Features.Books.Models.BookQuizzes;

public class AddBookQuizModel : IRequestModel
{
    public int BookId { get; set; }
    public short LessonId { get; set; }
    public string? Name { get; set; }
    public byte QuestionCount { get; set; }
    public byte OptionCount { get; set; }
    public OptionDto[] RightAnswers { get; set; } = [];
}