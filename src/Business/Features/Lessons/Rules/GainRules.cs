using Business.Features.Lessons.Models.Gains;

namespace Business.Features.Lessons.Rules;

public class GainRules(IGainDal gainDal) : IBusinessRule
{
    internal static Task GainShouldExists(GetGainModel model)
    {
        if (model == null) throw new BusinessException(Strings.DynamicNotFound, Strings.Gain);
        return Task.CompletedTask;
    }

    internal static Task GainShouldExists(Gain gain)
    {
        if (gain == null) throw new BusinessException(Strings.DynamicNotFound, Strings.Gain);
        return Task.CompletedTask;
    }

    internal async Task GainShouldExistsById(byte id)
    {
        var gain = await gainDal.IsExistsAsync(predicate: x => x.Id == id, enableTracking: false);
        if (!gain) throw new BusinessException(Strings.DynamicNotFound, Strings.Gain);
    }

    internal async Task GainShouldExistsAndActiveById(byte id)
    {
        var gain = await gainDal.IsExistsAsync(predicate: x => x.Id == id && x.IsActive, enableTracking: false);
        if (!gain) throw new BusinessException(Strings.DynamicNotFound, Strings.Gain);
    }
}