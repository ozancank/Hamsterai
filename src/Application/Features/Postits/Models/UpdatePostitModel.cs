namespace Application.Features.Postits.Models;

public sealed class UpdatePostitModel : IResponseModel
{
    public Guid Id { get; set; }
    public short LessonId { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Color { get; set; }
    public short SortNo { get; set; }
    public bool RemovePicture { get; set; }
}