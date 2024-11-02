using Domain.Constants;
using Domain.Entities.Core;

namespace Domain.Entities;

public class PackageCategory : BaseEntity<byte>
{
    public string? Name { get; set; }
    public byte SortNo { get; set; }
    public bool IsWebVisible { get; set; }
    public byte ParentId { get; set; }
    public string? Description { get; set; }
    public string? PictureUrl { get; set; }
    public string? Slug { get; set; }

    public virtual ICollection<Package> Packages { get; set; } = [];

    public PackageCategory() : base()
    { }

    public PackageCategory(byte id, bool isActive, long createUser, DateTime createDate, long updateUser, DateTime updateDate, string name)
       : base(id, isActive, createUser, createDate, updateUser, updateDate)
    {
        Name = name;
    }
}