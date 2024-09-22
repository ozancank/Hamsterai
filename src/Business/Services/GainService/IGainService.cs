using Business.Features.Lessons.Dto.Gain;
using Business.Features.Lessons.Models.Gain;

namespace Business.Services.GainService;

public interface IGainService : IBusinessService
{
    Task<GetGainModel> GetOrAddGainModelAsync(AddGainDto dto);
}