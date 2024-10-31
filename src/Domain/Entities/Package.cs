using Domain.Entities.Core;

namespace Domain.Entities;

public class Package : BaseEntity<byte>
{
    public string? Name { get; set; }
    public byte SortNo { get; set; }
    public bool IsWebVisible { get; set; }

    public virtual ICollection<RPackageGroup> RPackageLessons { get; set; } = [];
    public virtual ICollection<User> Users { get; set; } = [];
    public virtual ICollection<SchoolGroup> SchoolGroups { get; set; } = [];

    public Package() : base()
    { }

    public Package(byte id, bool isActive, long createUser, DateTime createDate, long updateUser, DateTime updateDate, string name)
       : base(id, isActive, createUser, createDate, updateUser, updateDate)
    {
        Name = name;
    }
}