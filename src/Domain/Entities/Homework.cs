using Domain.Entities.Core;

namespace Domain.Entities;

public class Homework : BaseEntity<string>
{
    public int? SchoolId { get; set; }
    public int? TeacherId { get; set; }
    public short LessonId { get; set; }
    public string? FilePath { get; set; }
    public int? ClassRoomId { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }

    public virtual User? User { get; set; }
    public virtual School? School { get; set; }
    public virtual Teacher? Teacher { get; set; }
    public virtual Lesson? Lesson { get; set; }
    public virtual ClassRoom? ClassRoom { get; set; }
    public virtual ICollection<HomeworkStudent> HomeworkStudents { get; set; } = [];
    public virtual ICollection<HomeworkUser> HomeworkUsers { get; set; } = [];

    public Homework() : base()
    { }

    public Homework(string id, bool isActive, long createUser, DateTime createDate, long updateUser, DateTime updateDate)
        : base(id, isActive, createUser, createDate, updateUser, updateDate)
    { }
}