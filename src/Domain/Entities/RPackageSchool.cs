using Domain.Entities.Core;

namespace Domain.Entities;

public class RPackageSchool : BaseEntity<Guid>
{
    public int SchoolId { get; set; }
    public short PackageId { get; set; }

    public virtual School? School { get; set; }
    public virtual Package? Package { get; set; }

    public RPackageSchool() : base()
    { }

    public RPackageSchool(Guid id, bool isActive, long createUser, DateTime createDate, long updateUser, DateTime updateDate, int schoolId, short packageId)
    : base(id, isActive, createUser, createDate, updateUser, updateDate)
    {
        SchoolId = schoolId;
        PackageId = packageId;
    }
}