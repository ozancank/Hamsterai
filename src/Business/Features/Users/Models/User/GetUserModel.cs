namespace Business.Features.Users.Models.User;

public class GetUserModel : IResponseModel
{
    public long Id { get; set; }
    public bool IsActive { get; set; }
    public string UserName { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Phone { get; set; }
    public string ProfileFileName { get; set; }
    public string Email { get; set; }
    public string FullName => $"{Name} {Surname}";
    public int? ConnectionId { get; set; }
    public int? SchoolId { get; set; }
    public byte Type { get; set; }
    public byte GroupId { get; set; }
    public int QuestionCount { get; set; }
    public IList<string> OperationClaims { get; set; }
}