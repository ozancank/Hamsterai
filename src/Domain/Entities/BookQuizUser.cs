using Domain.Entities.Core;

namespace Domain.Entities;

public class BookQuizUser : BaseEntity<Guid>
{
    public Guid BookQuizId { get; set; }
    public long UserId { get; set; }
    public string? Answers { get; set; }

    public virtual BookQuiz? BookQuiz { get; set; }
    public virtual User? User { get; set; }

    public BookQuizUser() : base()
    {
    }

    public BookQuizUser(Guid id, bool isActive, long createUser, DateTime createDate, long updateUser, DateTime updateDate)
        : base(id, isActive, createUser, createDate, updateUser, updateDate)
    {
    }
}