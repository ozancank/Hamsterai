﻿namespace Application.Features.Postits.Models;

public class PostitRequestModel : IRequestModel
{
    public short LessonId { get; set; } = 0;
    public DateTime? StartDate { get; set; } = null;
    public DateTime? EndDate { get; set; } = null;
}