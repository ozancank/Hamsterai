namespace Business.Features.Students.Models;

public class AddStudentModel : IRequestModel
{
    public string Name { get; set; }
    public string Surname { get; set; }
    public string StudentNo { get; set; }
    public string TcNo { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public string ClassRoomId { get; set; }
}