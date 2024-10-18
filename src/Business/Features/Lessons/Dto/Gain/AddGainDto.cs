using DataAccess.EF;

namespace Business.Features.Lessons.Dto.Gain;

public record AddGainDto(string GainName, byte LessonId, long UserId, HamsteraiDbContext Context = null);