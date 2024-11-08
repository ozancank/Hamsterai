namespace Application.Features.Schools.Models.Schools;

public class UpdateSchoolModel : IRequestModel
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? TaxNumber { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? AuthorizedName { get; set; }
    public string? AuthorizedPhone { get; set; }
    public string? AuthorizedEmail { get; set; }
    public DateTime LicenseEndDate { get; set; }
    public int UserCount { get; set; }
    public List<short> PackageIds { get; set; } = [];
}