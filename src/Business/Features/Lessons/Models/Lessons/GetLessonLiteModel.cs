namespace Business.Features.Lessons.Models.Lessons;

public sealed class GetLessonLiteModel : IResponseModel
{
    public byte Id { get; set; }
    public string Name { get; set; }
}