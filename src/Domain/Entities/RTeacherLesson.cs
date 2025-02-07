using Domain.Entities.Core;

namespace Domain.Entities;

public class RTeacherLesson : BaseEntity<Guid>
{
    public int TeacherId { get; set; }
    public short LessonId { get; set; }

    public virtual Teacher? Teacher { get; set; }
    public virtual Lesson? Lesson { get; set; }

    public RTeacherLesson() : base()
    { }

    public RTeacherLesson(Guid id, bool isActive, long createUser, DateTime createDate, long updateUser, DateTime updateDate, byte teacherId, byte lessonId)
        : base(id, isActive, createUser, createDate, updateUser, updateDate)
    {
        TeacherId = teacherId;
        LessonId = lessonId;
    }
}