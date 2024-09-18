namespace Business.Features.Lessons.Models.Lessons;

public sealed class AddLessonModel : IResponseModel
{
    public string Name { get; set; }
    public byte SortNo { get; set; }
}