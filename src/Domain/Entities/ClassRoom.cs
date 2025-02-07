using Domain.Entities.Core;

namespace Domain.Entities;

public class ClassRoom : BaseEntity<int>
{
    public short No { get; set; }
    public string? Branch { get; set; }
    public int SchoolId { get; set; }
    public short? PackageId { get; set; }

    public virtual School? School { get; set; }
    public virtual Package? Package { get; set; }
    public virtual ICollection<RTeacherClassRoom> TeacherClassRooms { get; set; } = [];
    public virtual ICollection<Student> Students { get; set; } = [];
    public virtual ICollection<Homework> Homeworks { get; set; } = [];
    public virtual ICollection<RBookClassRoom> BookClassRooms { get; set; } = [];

    public ClassRoom() : base()
    { }

    public ClassRoom(int id, bool isActive, long createUser, DateTime createDate, long updateUser, DateTime updateDate)
        : base(id, isActive, createUser, createDate, updateUser, updateDate)
    { }
}