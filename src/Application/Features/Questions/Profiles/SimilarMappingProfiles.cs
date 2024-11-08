using Application.Features.Questions.Models.Similars;

namespace Application.Features.Questions.Profiles;

public class SimilarMappingProfiles : Profile
{
    public SimilarMappingProfiles()
    {
        CreateMap<Similar, GetSimilarModel>()
            .ForMember(dest => dest.LessonName, opt => opt.MapFrom(src => src.Lesson!.Name))
            .ForMember(dest => dest.GainName, opt => opt.MapFrom(src => src.Gain!.Name));
        CreateMap<IPaginate<GetSimilarModel>, PageableModel<GetSimilarModel>>();

        CreateMap<AddSimilarModel, Similar>();
    }
}