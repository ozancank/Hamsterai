namespace Application.Features.Students.Models;

public class GetStudentGainsModel : IResponseModel
{
    public Dictionary<string, int> ForLessons { get; set; } = [];
    public Dictionary<string, int> ForGains { get; set; } = [];
    public Dictionary<string, Dictionary<string, int>> ForLessonGains { get; set; } = [];
    public Dictionary<string, int> Info { get; set; } = [];
}