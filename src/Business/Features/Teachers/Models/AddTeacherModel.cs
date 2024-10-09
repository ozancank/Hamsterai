namespace Business.Features.Teachers.Models;

public class AddTeacherModel : IRequestModel
{
    public string Name { get; set; }
    public string Surname { get; set; }
    public string TcNo { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public string Branch { get; set; }
}