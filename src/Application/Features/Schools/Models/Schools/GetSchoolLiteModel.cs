using Application.Features.Packages.Models.Packages;
using Application.Features.Users.Models.User;

namespace Application.Features.Schools.Models.Schools;

public sealed class GetSchoolLiteModel : IResponseModel
{
    public int Id { get; set; }
    public bool IsActive { get; set; }
    public string? Name { get; set; }
    public string? City { get; set; }
    public string? TaxNumber { get; set; }
    public string? Address { get; set; }
    public string? AuthorizedName { get; set; }
    public string? AuthorizedPhone { get; set; }
    public string? AuthorizedEmail { get; set; }
    public int UserCount { get; set; }
    public int TeacherCount { get; set; }
    public int StudentCount { get; set; }
    public long UserId { get; set; }
    public DateTime LicenseEndDate { get; set; }

    public List<short> PackageIds { get; set; } = [];
    public List<GetPackageLiteModel> Packages { get; set; } = [];
}