using Application.Features.Questions.Models.Similars;

namespace Application.Features.Questions.Profiles;

public class SimilarMappingProfiles : Profile
{
    public SimilarMappingProfiles()
    {
        CreateMap<Similar, GetSimilarModel>()
            .ForMember(dest => dest.LessonName, opt => opt.MapFrom(src => src.Lesson != null ? src.Lesson.Name : default))
            .ForMember(dest => dest.GainName, opt => opt.MapFrom(src => src.Gain != null ? src.Gain.Name : default));
        CreateMap<IPaginate<GetSimilarModel>, PageableModel<GetSimilarModel>>();

        CreateMap<AddSimilarModel, Similar>();
    }
}