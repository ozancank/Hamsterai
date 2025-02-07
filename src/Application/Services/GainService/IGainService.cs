using Application.Features.Lessons.Dto;
using Application.Features.Lessons.Models.Gains;

namespace Application.Services.GainService;

public interface IGainService : IBusinessService
{
    Task<GetGainModel?> GetOrAddGain(AddGainDto dto);
}