namespace Application.Features.Lessons.Models.Lessons;

public sealed class UpdateLessonModel : IResponseModel
{
    public short Id { get; set; }
    public string? Name { get; set; }
    public short SortNo { get; set; }
}