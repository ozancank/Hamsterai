namespace Application.Features.Books.Models.BookQuizzes;

public class BookQuizRequestModel : IRequestModel
{
    public int BookId { get; set; }
    public short LessonId { get; set; } = 0;
}