using Domain.Constants;
using Domain.Entities.Core;

namespace Domain.Entities;

public class Quiz : BaseEntity<string>
{
    public long UserId { get; set; }
    public byte LessonId { get; set; }
    public int TimeSecond { get; set; }
    public QuizStatus Status { get; set; }
    public byte CorrectCount { get; set; }
    public byte WrongCount { get; set; }
    public byte EmptyCount { get; set; }
    public double SuccessRate { get; set; }

    public virtual User? User { get; set; }
    public virtual Lesson? Lesson { get; set; }
    public virtual ICollection<QuizQuestion> QuizQuestions { get; set; } = [];

    public Quiz() : base()
    { }

    public Quiz(string id, bool isActive, long createUser, DateTime createDate, long updateUser, DateTime updateDate)
        : base(id, isActive, createUser, createDate, updateUser, updateDate)
    { }
}