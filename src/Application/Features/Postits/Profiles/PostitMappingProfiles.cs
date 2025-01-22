using Application.Features.Postits.Models;

namespace Application.Features.Postits.Profiles;

public class PostitMappingProfiles : Profile
{
    public PostitMappingProfiles()
    {
        CreateMap<Postit, GetPostitModel>()
            .ForMember(dest => dest.LessonName, opt => opt.MapFrom(src => src.Lesson != null ? src.Lesson.Name : string.Empty));
        CreateMap<IPaginate<GetPostitModel>, PageableModel<GetPostitModel>>();

        CreateMap<AddPostitModel, Postit>();
        CreateMap<UpdatePostitModel, Postit>();
    }
}