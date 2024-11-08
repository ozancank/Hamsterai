namespace Application.Features.Lessons.Models.Lessons;

public sealed class AddLessonModel : IResponseModel
{
    public string? Name { get; set; }
    public short SortNo { get; set; }
}