using Domain.Entities.Core;

namespace Domain.Entities;

public class ClassRoom : BaseEntity<int>
{
    public short No { get; set; }
    public string Branch { get; set; }
    public int SchoolId { get; set; }

    public virtual School School { get; set; }
    public virtual ICollection<TeacherClassRoom> TeacherClassRooms { get; set; }
    public virtual ICollection<Student> Students { get; set; }

    public ClassRoom() : base()
    {
        TeacherClassRooms = [];
        Students = [];
    }

    public ClassRoom(int id, bool isActive, long createUser, DateTime createDate, long updateUser, DateTime updateDate)
        : base(id, isActive, createUser, createDate, updateUser, updateDate)
    {
        TeacherClassRooms = [];
        Students = [];
    }
}