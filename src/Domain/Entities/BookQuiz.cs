using Domain.Entities.Core;

namespace Domain.Entities;

public class BookQuiz : BaseEntity<Guid>
{
    public int BookId { get; set; }
    public short LessonId { get; set; }
    public string? Name { get; set; }
    public byte QuestionCount { get; set; }
    public byte OptionCount { get; set; }    
    public string? Answers { get; set; }

    public virtual Book? Book { get; set; }
    public virtual Lesson? Lesson { get; set; }
    public virtual ICollection<BookQuizUser> BookQuizUsers { get; set; } = [];

    public BookQuiz() : base()
    {
    }

    public BookQuiz(Guid id, bool isActive, long createUser, DateTime createDate, long updateUser, DateTime updateDate)
        : base(id, isActive, createUser, createDate, updateUser, updateDate)
    {
    }
}