namespace Application.Features.Books.Models.Books;

public class AddBookModel : IRequestModel
{
    public int SchoolId { get; set; }
    public short LessonId { get; set; }
    public short PublisherId { get; set; }
    public string? Name { get; set; }
    public short Year { get; set; }

    public IFormFile? File { get; set; }
    public List<int> ClassRoomIds { get; set; } = [];
}