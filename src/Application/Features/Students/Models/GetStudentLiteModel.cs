namespace Application.Features.Students.Models;

public class GetStudentLiteModel : IResponseModel
{
    public int Id { get; set; }
    public bool IsActive { get; set; }
    public string? Name { get; set; }
    public string? Surname { get; set; }
    public string? StudentNo { get; set; }
    public int ClassRoomId { get; set; }
    public string? FullName { get; set; }
    public int SchoolId { get; set; }    
}