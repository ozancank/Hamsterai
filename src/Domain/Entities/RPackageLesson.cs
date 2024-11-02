using Domain.Entities.Core;

namespace Domain.Entities;

public class RPackageLesson : BaseEntity<Guid>
{
    public short PackageId { get; set; }
    public byte LessonId { get; set; }

    public virtual Package? Package { get; set; }
    public virtual Lesson? Lesson { get; set; }

    public RPackageLesson() : base()
    { }

    public RPackageLesson(Guid id, bool isActive, long createUser, DateTime createDate, long updateUser, DateTime updateDate, short packageId, byte lessonId)
    : base(id, isActive, createUser, createDate, updateUser, updateDate)
    {
        PackageId = packageId;
        LessonId = lessonId;
    }
}