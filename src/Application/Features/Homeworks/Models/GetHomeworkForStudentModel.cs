﻿namespace Application.Features.Homeworks.Models;

public class GetHomeworkForStudentModel : IResponseModel
{
    public string? Id { get; set; }
    public bool IsActive { get; set; }
    public long CreateUser { get; set; }
    public DateTime CreateDate { get; set; }
    public long UpdateUser { get; set; }
    public DateTime UpdateDate { get; set; }
    public int? SchoolId { get; set; }
    public string? SchoolName { get; set; }
    public int? TeacherId { get; set; }
    public string? TeacherName { get; set; }
    public short LessonId { get; set; }
    public string? LessonName { get; set; }
    public string? FilePath { get; set; }
    public int? ClassRoomId { get; set; }
    public string? ClassName { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
}