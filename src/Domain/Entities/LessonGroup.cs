using Domain.Entities.Core;

namespace Domain.Entities;

public class LessonGroup : BaseEntity<Guid>
{
    public byte GroupId { get; set; }
    public byte LessonId { get; set; }

    public virtual Group Group { get; set; }
    public virtual Lesson Lesson { get; set; }

    public LessonGroup() : base()
    { }

    public LessonGroup(Guid id, bool isActive, long createUser, DateTime createDate, long updateUser, DateTime updateDate, byte groupId, byte lessonId)
    : base(id, isActive, createUser, createDate, updateUser, updateDate)
    {
        GroupId = groupId;
        LessonId = lessonId;
    }
}