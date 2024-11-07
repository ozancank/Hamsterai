namespace Business.Features.Lessons.Models.Gains;

public class GetGainModel : IResponseModel
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public short LessonId { get; set; }
    public string? LessonName { get; set; }
}