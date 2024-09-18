using Domain.Entities.Core;

namespace Domain.Entities;

public class Group : BaseEntity<byte>
{
    public string Name { get; set; }

    public virtual ICollection<LessonGroup> LessonGroups { get; set; } = [];

    public Group() : base()
    {
        LessonGroups = [];
    }

    public Group(byte id, bool isActive, long createUser, DateTime createDate, long updateUser, DateTime updateDate, string name)
       : base(id, isActive, createUser, createDate, updateUser, updateDate)
    {
        Name = name;
        LessonGroups = [];
    }
}