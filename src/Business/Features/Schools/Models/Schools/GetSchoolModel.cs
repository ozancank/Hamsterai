using Business.Features.Packages.Models;
using Business.Features.Users.Models.User;

namespace Business.Features.Schools.Models.Schools;

public sealed class GetSchoolModel : IResponseModel
{
    public int Id { get; set; }
    public bool IsActive { get; set; }
    public long CreateUser { get; set; }
    public DateTime CreateDate { get; set; }
    public long UpdateUser { get; set; }
    public DateTime UpdateDate { get; set; }
    public string? Name { get; set; }
    public string? City { get; set; }
    public string? TaxNumber { get; set; }
    public string? Address { get; set; }
    public string? AuthorizedName { get; set; }
    public string? AuthorizedPhone { get; set; }
    public string? AuthorizedEmail { get; set; }
    public DateTime LicenseEndDate { get; set; }
    public int UserCount { get; set; }
    public int TeacherCount { get; set; }
    public int StudentCount { get; set; }

    public List<byte> PackageIds { get; set; } = [];
    public List<GetPackageLiteModel> Packages { get; set; } = [];
    public List<GetUserModel> Users { get; set; } = [];
    public List<Teacher> Teachers { get; set; } = [];
    public List<ClassRoom> ClassRooms { get; set; } = [];
}