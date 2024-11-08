namespace Application.Features.Students.Models;

public class StudentGainsRequestModel : IRequestModel
{
    public int StudentId { get; set; } = 0;
    public DateTime? StartDate { get; set; } = null;
    public DateTime? EndDate { get; set; } = null;
}