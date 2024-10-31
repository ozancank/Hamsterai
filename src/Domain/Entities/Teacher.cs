using Domain.Entities.Core;

namespace Domain.Entities;

public class Teacher : BaseEntity<int>
{
    public string? Name { get; set; }
    public string? Surname { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Branch { get; set; }
    public int SchoolId { get; set; }

    public virtual School? School { get; set; }
    public virtual ICollection<RTeacherLesson> RTeacherLessons { get; set; } = [];
    public virtual ICollection<RTeacherClassRoom> RTeacherClassRooms { get; set; } = [];
    public virtual ICollection<Homework> Homeworks { get; set; } = [];

    public Teacher() : base()
    { }

    public Teacher(int id, bool isActive, long createUser, DateTime createDate, long updateUser, DateTime updateDate)
        : base(id, isActive, createUser, createDate, updateUser, updateDate)
    { }
}