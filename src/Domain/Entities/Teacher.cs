using Domain.Entities.Core;

namespace Domain.Entities;

public class Teacher : BaseEntity<int>
{
    public string Name { get; set; }
    public string Surname { get; set; }
    public string TcNo { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public string Branch { get; set; }
    public int SchoolId { get; set; }

    public virtual School School { get; set; }
    public virtual ICollection<TeacherLesson> TeacherLessons { get; set; }
    public virtual ICollection<TeacherClassRoom> TeacherClassRooms { get; set; }

    public Teacher() : base()
    {
        TeacherClassRooms = [];
    }

    public Teacher(int id, bool isActive, long createUser, DateTime createDate, long updateUser, DateTime updateDate)
        : base(id, isActive, createUser, createDate, updateUser, updateDate)
    {
        TeacherClassRooms = [];
    }
}