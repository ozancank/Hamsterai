using Domain.Entities.Core;

namespace Domain.Entities;

public class TeacherLesson : BaseEntity<Guid>
{
    public int TeacherId { get; set; }
    public byte LessonId { get; set; }

    public virtual Teacher Teacher { get; set; }
    public virtual Lesson Lesson { get; set; }

    public TeacherLesson() : base()
    {
    }

    public TeacherLesson(Guid id, bool isActive, long createUser, DateTime createDate, long updateUser, DateTime updateDate, byte teacherId, byte lessonId)
        : base(id, isActive, createUser, createDate, updateUser, updateDate)
    {
        TeacherId = teacherId;
        LessonId = lessonId;
    }
}