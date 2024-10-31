using Domain.Entities.Core;

namespace Domain.Entities;

public class RPackageGroup : BaseEntity<Guid>
{
    public byte GroupId { get; set; }
    public byte LessonId { get; set; }

    public virtual Package Group { get; set; }
    public virtual Lesson Lesson { get; set; }

    public RPackageGroup() : base()
    { }

    public RPackageGroup(Guid id, bool isActive, long createUser, DateTime createDate, long updateUser, DateTime updateDate, byte groupId, byte lessonId)
    : base(id, isActive, createUser, createDate, updateUser, updateDate)
    {
        GroupId = groupId;
        LessonId = lessonId;
    }
}