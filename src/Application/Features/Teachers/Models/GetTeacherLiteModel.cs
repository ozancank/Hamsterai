namespace Application.Features.Teachers.Models;

public sealed class GetTeacherLiteModel : IResponseModel
{
    public int Id { get; set; }
    public bool IsActive { get; set; }
    public string? Name { get; set; }
    public string? Surname { get; set; }
    public string? TcNo { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Branch { get; set; }
    public int SchoolId { get; set; }
    public string? SchoolName { get; set; }
    public string? FullName { get; set; }
    public long UserId { get; set; }
}