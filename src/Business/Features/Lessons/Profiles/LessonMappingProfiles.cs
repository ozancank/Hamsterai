using Business.Features.Lessons.Models.Lessons;

namespace Business.Features.Lessons.Profiles;

public class LessonMappingProfiles : Profile
{
    public LessonMappingProfiles()
    {
        CreateMap<Lesson, GetLessonModel>()
            .ForMember(dest => dest.Groups, opt => opt.MapFrom(src => src.LessonGroups.Where(x => x.Group.IsActive).Select(x => x.Group).OrderBy(x => x.Name)));
        CreateMap<Lesson, GetLessonLiteModel>();
        CreateMap<IPaginate<GetLessonModel>, PageableModel<GetLessonModel>>();

        CreateMap<AddLessonModel, Lesson>();
        CreateMap<UpdateLessonModel, Lesson>();
    }
}