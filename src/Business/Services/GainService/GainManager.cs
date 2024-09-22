using Business.Features.Lessons.Dto.Gain;
using Business.Features.Lessons.Models.Gain;
using Business.Features.Lessons.Rules;

namespace Business.Services.GainService;

public class GainManager(IMapper mapper,
                         IGainDal gainDal,
                         LessonRules lessonRules) : IGainService
{
    public async Task<GetGainModel> GetOrAddGainModelAsync(AddGainDto dto)
    {
        if(dto.GainName.IsEmpty()) return null;

        await lessonRules.LessonShouldExistsById(dto.LessonId);

        var gain = await gainDal.GetAsync(
            predicate: x => x.Name == dto.GainName && x.LessonId == dto.LessonId,
            include: x => x.Include(u => u.Lesson),
            enableTracking: false);

        if (gain == null)
        {
            gain = new Gain
            {
                Name = dto.GainName,
                LessonId = dto.LessonId
            };
            gain = await gainDal.AddAsyncCallback(gain);
        }

        var result = mapper.Map<GetGainModel>(gain);
        return result;
    }
}