using Domain.Entities.Core;

namespace Domain.Entities;

public class Gain : BaseEntity<int>
{
    public string Name { get; set; }
    public byte LessonId { get; set; }

    public virtual Lesson Lesson { get; set; }
    public virtual ICollection<Question> Questions { get; set; }
    public virtual ICollection<Similar> SimilarQuestions { get; set; }

    public Gain()
    {
        Questions = [];
        SimilarQuestions = [];
    }
}