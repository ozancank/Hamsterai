using Business.Features.Lessons.Dto.Gain;
using Business.Features.Lessons.Models.Gains;
using Business.Features.Lessons.Rules;
using Business.Services.CommonService;

namespace Business.Services.GainService;

public class GainManager(IMapper mapper,
                         IGainDal gainDal,
                         LessonRules lessonRules) : IGainService
{
    public async Task<GetGainModel> GetOrAddGain(AddGainDto dto)
    {
        if (dto.GainName.IsEmpty()) return null;

        await lessonRules.LessonShouldExistsById(dto.LessonId);

        var gain = await gainDal.GetAsync(
            predicate: x => x.Name == dto.GainName && x.LessonId == dto.LessonId,
            include: x => x.Include(u => u.Lesson),
            enableTracking: false);

        var date = DateTime.Now;

        if (gain == null)
        {
            gain = new Gain
            {
                Id = await gainDal.GetNextPrimaryKeyAsync(x => x.Id),
                IsActive = true,
                CreateUser = dto.UserId,
                CreateDate = date,
                UpdateUser = dto.UserId,
                UpdateDate = date,
                Name = dto.GainName,
                LessonId = dto.LessonId
            };
            gain = await gainDal.AddAsyncCallback(gain);
        }

        var result = mapper.Map<GetGainModel>(gain);
        return result;
    }
}