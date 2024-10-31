namespace Business.Features.Lessons.Models.Lessons;

public sealed class UpdateLessonModel : IResponseModel
{
    public byte Id { get; set; }
    public string? Name { get; set; }
    public byte SortNo { get; set; }
}