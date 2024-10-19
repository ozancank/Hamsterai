using Domain.Entities.Core;

namespace Domain.Entities;

public class SchoolGroup : BaseEntity<Guid>
{
    public int SchoolId { get; set; }
    public byte GroupId { get; set; }

    public virtual School School { get; set; }
    public virtual Group Group { get; set; }

    public SchoolGroup() : base()
    { }

    public SchoolGroup(Guid id, bool isActive, long createUser, DateTime createDate, long updateUser, DateTime updateDate, int schoolId, byte groupId)
    : base(id, isActive, createUser, createDate, updateUser, updateDate)
    {
        SchoolId = schoolId;
        GroupId = groupId;
    }
}