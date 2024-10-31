using Domain.Entities.Core;

namespace Domain.Entities;

public class Student : BaseEntity<int>
{
    public string? Name { get; set; }
    public string? Surname { get; set; }
    public string? StudentNo { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public int ClassRoomId { get; set; }

    public virtual ClassRoom? ClassRoom { get; set; }
    public virtual ICollection<Teacher> Teachers { get; set; } = [];
    public virtual ICollection<HomeworkStudent> HomeworkStudents { get; set; } = [];

    public Student() : base()
    { }

    public Student(int id, bool isActive, long createUser, DateTime createDate, long updateUser, DateTime updateDate)
        : base(id, isActive, createUser, createDate, updateUser, updateDate)
    { }
}