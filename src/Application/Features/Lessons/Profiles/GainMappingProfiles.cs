using Application.Features.Lessons.Models.Gains;
using Application.Features.Lessons.Models.Lessons;

namespace Application.Features.Lessons.Profiles;

public class GainMappingProfiles : Profile
{
    public GainMappingProfiles()
    {
        CreateMap<Gain, GetGainModel>()
            .ForMember(dest => dest.LessonName, opt => opt.MapFrom(src => src.Lesson!.Name));
        CreateMap<IPaginate<GetLessonModel>, PageableModel<GetLessonModel>>();
    }
}