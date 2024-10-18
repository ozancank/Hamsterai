using OCK.Core.Interfaces;

namespace Infrastructure.AI;

public class QuizApiModel : IRequestModel
{
    public List<string> QuestionImages { get; set; }
    public List<string> QuestionTexts { get; set; }
    public string LessonName { get; set; }
    public long UserId { get; set; }
}