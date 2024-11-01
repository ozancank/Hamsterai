using Domain.Constants;
using Domain.Entities.Core;

namespace Domain.Entities;

public class Package : BaseEntity<byte>
{
    public string? Name { get; set; }
    public byte SortNo { get; set; }
    public bool IsWebVisible { get; set; }
    public double UnitPrice { get; set; }
    public double? UnitOldPrice { get; set; }
    public double TaxRatio { get; set; }
    public double TaxAmount { get; set; }
    public double? TaxOldAmount { get; set; }
    public double Amount { get; set; }
    public double? OldAmount { get; set; }
    public PaymentRenewalPeriod PaymentRenewalPeriod { get; set; }
    public string? Description { get; set; }
    public string? PictureUrl { get; set; }

    public virtual ICollection<PackageUser> PackageUsers { get; set; } = [];
    public virtual ICollection<RPackageLesson> RPackageLessons { get; set; } = [];
    public virtual ICollection<RPackageSchool> RPackageSchools { get; set; } = [];
    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = [];

    public Package() : base()
    { }

    public Package(byte id, bool isActive, long createUser, DateTime createDate, long updateUser, DateTime updateDate, string name)
       : base(id, isActive, createUser, createDate, updateUser, updateDate)
    {
        Name = name;
    }
}