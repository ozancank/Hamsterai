namespace Business.Features.Schools.Models.Teachers;

public class UpdateTeacherModel : IRequestModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string TcNo { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public string Branch { get; set; }
}