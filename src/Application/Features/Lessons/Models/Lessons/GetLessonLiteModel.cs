namespace Application.Features.Lessons.Models.Lessons;

public sealed class GetLessonLiteModel : IResponseModel
{
    public short Id { get; set; }
    public string? Name { get; set; }
}