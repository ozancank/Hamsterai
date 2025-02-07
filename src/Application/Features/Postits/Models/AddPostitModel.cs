namespace Application.Features.Postits.Models;

public sealed class AddPostitModel : IResponseModel
{
    public short LessonId { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Color { get; set; }
    public short SortNo { get; set; }

    public string? PictureBase64 { get; set; }
    public string? PictureFileName { get; set; }
}