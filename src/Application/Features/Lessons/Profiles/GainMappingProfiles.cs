using Application.Features.Lessons.Models.Gains;
using Application.Features.Lessons.Models.Lessons;

namespace Application.Features.Lessons.Profiles;

public class GainMappingProfiles : Profile
{
    public GainMappingProfiles()
    {
        CreateMap<Gain, GetGainModel>()
            .ForMember(dest => dest.LessonName, opt => opt.MapFrom(src => src.Lesson != null ? src.Lesson.Name : default));
        CreateMap<IPaginate<GetGainModel>, PageableModel<GetGainModel>>();
    }
}