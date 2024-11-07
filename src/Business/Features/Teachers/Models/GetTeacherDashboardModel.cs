namespace Business.Features.Teachers.Models;

public class GetTeacherDashboardModel : IResponseModel
{
    public int TeacherId { get; set; }
    public long UserId { get; set; }
    public int SchoolId { get; set; }
    public string? SchoolName { get; set; }
    public string? TeacherName { get; set; }
    public int TotalStudentCount { get; set; }
    public int TotalClassRoomCount { get; set; }
    public int TotalLessonCount { get; set; }
    public int TotalQuestionCount { get; set; }
    public int SendQuestionCount { get; set; }
    public int SendSimilarCount { get; set; }

    public Dictionary<string, int> SendQuestionByLesson { get; set; } = [];
    public Dictionary<string, int> SendQuestionByPackage { get; set; } = [];
    public Dictionary<string, int> SendQuestionByClassRoom { get; set; } = [];
    public Dictionary<string, int> SendQuestionByDay { get; set; } = [];

    public Dictionary<string, int> SendSimilarByLesson { get; set; } = [];
    public Dictionary<string, int> SendSimilarByPackage { get; set; } = [];
    public Dictionary<string, int> SendSimilarByClassRoom { get; set; } = [];
    public Dictionary<string, int> SendSimilarByDay { get; set; } = [];

    public Dictionary<string, int> TotalQuestionByLesson { get; set; } = [];
    public Dictionary<string, int> TotalQuestionByPackage { get; set; } = [];
    public Dictionary<string, int> TotalQuestionByClassRoom { get; set; } = [];
    public Dictionary<string, int> TotalQuestionByDay { get; set; } = [];
}