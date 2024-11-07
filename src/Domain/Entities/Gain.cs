using Domain.Entities.Core;

namespace Domain.Entities;

public class Gain : BaseEntity<int>
{
    public string? Name { get; set; }
    public short LessonId { get; set; }

    public virtual Lesson? Lesson { get; set; }
    public virtual ICollection<Question> Questions { get; set; } = [];
    public virtual ICollection<Similar> SimilarQuestions { get; set; } = [];
    public virtual ICollection<QuizQuestion> QuizQuestions { get; set; } = [];

    public Gain() : base()
    { }

    public Gain(int id, bool isActive, long createUser, DateTime createDate, long updateUser, DateTime updateDate)
        : base(id, isActive, createUser, createDate, updateUser, updateDate)
    { }
}