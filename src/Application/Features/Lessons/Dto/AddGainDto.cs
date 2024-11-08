using DataAccess.EF;

namespace Application.Features.Lessons.Dto;

public record AddGainDto(string? GainName, short LessonId, long UserId, HamsteraiDbContext? Context = null);