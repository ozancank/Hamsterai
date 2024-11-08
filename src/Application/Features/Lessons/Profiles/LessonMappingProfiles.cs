using Application.Features.Lessons.Models.Lessons;

namespace Application.Features.Lessons.Profiles;

public class LessonMappingProfiles : Profile
{
    public LessonMappingProfiles()
    {
        CreateMap<Lesson, GetLessonModel>()
            .ForMember(dest => dest.Packages, opt => opt.MapFrom(src => src.RPackageLessons.Where(x => x.Package!.IsActive).Select(x => x.Package).OrderBy(x => x!.Name)));
        CreateMap<Lesson, GetLessonLiteModel>();
        CreateMap<IPaginate<GetLessonModel>, PageableModel<GetLessonModel>>();

        CreateMap<AddLessonModel, Lesson>();
        CreateMap<UpdateLessonModel, Lesson>();
    }
}