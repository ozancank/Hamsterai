using Business.Features.Lessons.Dto.Gain;
using Business.Features.Lessons.Models.Gains;

namespace Business.Services.GainService;

public interface IGainService : IBusinessService
{
    Task<GetGainModel> GetOrAddGain(AddGainDto dto);
}