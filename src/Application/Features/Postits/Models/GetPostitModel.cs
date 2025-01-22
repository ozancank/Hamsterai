namespace Application.Features.Postits.Models;

public class GetPostitModel : IResponseModel
{
    public Guid Id { get; set; }
    public bool IsActive { get; set; }
    public long CreateUser { get; set; }
    public DateTime CreateDate { get; set; }
    public long? UpdateUser { get; set; }
    public DateTime? UpdateDate { get; set; }
    public short LessonId { get; set; }
    public string? LessonName { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Color { get; set; }
    public short SortNo { get; set; }
    public string? PictureFileName { get; set; }
}