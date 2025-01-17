namespace Application.Features.Books.Models.Books;

public class GetBookModel : IResponseModel
{
    public int Id { get; set; }
    public bool IsActive { get; set; }
    public long CreateUser { get; set; }
    public DateTime CreateDate { get; set; }
    public long UpdateUser { get; set; }
    public DateTime UpdateDate { get; set; }
    public int SchoolId { get; set; }
    public string? SchoolName { get; set; }
    public short LessonId { get; set; }
    public string? LessonName { get; set; }
    public short PublisherId { get; set; }
    public string? PublisherName { get; set; }
    public string? Name { get; set; }
    public short? Year { get; set; }

    public List<string> ClassRoomNames { get; set; } = [];
}