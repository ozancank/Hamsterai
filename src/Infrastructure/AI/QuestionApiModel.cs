using OCK.Core.Interfaces;

namespace Infrastructure.AI;

public class QuestionApiModel : IRequestModel
{
    public Guid Id { get; set; }
    public string Base64 { get; set; }
    public string LessonName { get; set; }
    public long UserId { get; set; }
}