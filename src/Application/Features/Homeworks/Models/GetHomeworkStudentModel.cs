namespace Application.Features.Homeworks.Models;

public class GetHomeworkStudentModel : IResponseModel
{
    public string? Id { get; set; }
    public bool IsActive { get; set; }
    public long CreateUser { get; set; }
    public DateTime CreateDate { get; set; }
    public long UpdateUser { get; set; }
    public DateTime UpdateDate { get; set; }
    public string? TeacherName { get; set; }
    public int StudentId { get; set; }
    public string? StudentName { get; set; }
    public string? HomeworkId { get; set; }
    public string? AnswerPath { get; set; }
    public HomeworkStatus Status { get; set; }

    public GetHomeworkForStudentModel? Homework { get; set; }
}