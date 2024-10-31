using Domain.Entities.Core;

namespace Domain.Entities;

public class RPackageUser : BaseEntity<Guid>
{
    public byte PackageId { get; set; }
    public long UserId { get; set; }
    public virtual Package? Package { get; set; }
    public virtual User? User { get; set; }

    public RPackageUser() : base()
    { }

    public RPackageUser(Guid id, bool isActive, long createUser, DateTime createDate, long updateUser, DateTime updateDate, byte packageId, long userId)
        : base(id, isActive, createUser, createDate, updateUser, updateDate)
    {
        PackageId = packageId;
        UserId = userId;
    }
}