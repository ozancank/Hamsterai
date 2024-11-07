using DataAccess.EF;

namespace Business.Features.Lessons.Dto.Gain;

public record AddGainDto(string? GainName, short LessonId, long UserId, HamsteraiDbContext? Context = null);