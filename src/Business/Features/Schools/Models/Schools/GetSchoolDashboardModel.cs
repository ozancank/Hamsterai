namespace Business.Features.Schools.Models.Schools;

public class GetSchoolDashboardModel : IResponseModel
{
    public int SchoolId { get; set; }
    public long UserId { get; set; }
    public string SchoolName { get; set; }
    public DateTime LicenceEndDate { get; set; }
    public int RemainingDay { get; set; }
    public int MaxUserCount { get; set; }

    public int TotalUserCount { get; set; }
    public int TotalTeacherCount { get; set; }
    public int TotalStudentCount { get; set; }
    public int TotalClassRoomCount { get; set; }
    public int TotalQuestionCount { get; set; }
    public int SendQuestionCount { get; set; }
    public int SendSimilarCount { get; set; }

    public Dictionary<string, int> SendQuestionByLesson { get; set; }
    public Dictionary<string, int> SendQuestionByGroup { get; set; }
    public Dictionary<string, int> SendQuestionByClassRoom { get; set; }

    public Dictionary<string, int> SendSimilarByLesson { get; set; }
    public Dictionary<string, int> SendSimilarByGroup { get; set; }
    public Dictionary<string, int> SendSimilarByClassRoom { get; set; }

    public Dictionary<string, int> TotalQuestionByLesson { get; set; }
    public Dictionary<string, int> TotalQuestionByGroup { get; set; }
    public Dictionary<string, int> TotalQuestionByClassRoom { get; set; }
}