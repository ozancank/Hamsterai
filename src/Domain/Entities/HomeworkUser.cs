using Domain.Constants;
using Domain.Entities.Core;

namespace Domain.Entities;

public class HomeworkUser : BaseEntity<string>
{
    public long UserId { get; set; }
    public string? HomeworkId { get; set; }
    public string? AnswerPath { get; set; }
    public HomeworkStatus Status { get; set; }

    public virtual User? User { get; set; }
    public virtual Homework? Homework { get; set; }

    public HomeworkUser() : base()
    { }

    public HomeworkUser(string id, bool isActive, long createUser, DateTime createDate, long updateUser, DateTime updateDate)
        : base(id, isActive, createUser, createDate, updateUser, updateDate)
    { }
}