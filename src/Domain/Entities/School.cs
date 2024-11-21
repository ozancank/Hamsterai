using Domain.Entities.Core;

namespace Domain.Entities;

public class School : BaseEntity<int>
{
    public string? Name { get; set; }
    public string? TaxNumber { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? AuthorizedName { get; set; }
    public string? AuthorizedPhone { get; set; }
    public string? AuthorizedEmail { get; set; }
    public DateTime LicenseEndDate { get; set; }
    public int UserCount { get; set; }
    public bool AccessStundents { get; set; }

    public virtual ICollection<User> Users { get; set; } = [];
    public virtual ICollection<Teacher> Teachers { get; set; } = [];
    public virtual ICollection<ClassRoom> ClassRooms { get; set; } = [];
    public virtual ICollection<Homework> Homeworks { get; set; } = [];
    public virtual ICollection<RPackageSchool> RPackageSchools { get; set; } = [];

    public School() : base()
    { }

    public School(byte id, bool isActive, long createUser, DateTime createDate, long updateUser, DateTime updateDate)
        : base(id, isActive, createUser, createDate, updateUser, updateDate)
    { }
}