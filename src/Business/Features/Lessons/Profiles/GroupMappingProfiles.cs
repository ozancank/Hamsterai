using Business.Features.Lessons.Models.Groups;

namespace Business.Features.Lessons.Profiles;

public class GroupMappingProfiles : Profile
{
    public GroupMappingProfiles()
    {
        CreateMap<Package, GetGroupModel>()
            .ForMember(dest => dest.Lessons, opt => opt.MapFrom(src => src.RPackageLessons.Where(x => x.Lesson.IsActive).Select(x => x.Lesson).OrderBy(x => x.Name)))
            .ForMember(dest => dest.LessonIds, opt => opt.MapFrom(src => src.RPackageLessons.Select(x => x.LessonId)));
        CreateMap<Package, GetGroupLiteModel>();
        CreateMap<IPaginate<GetGroupModel>, PageableModel<GetGroupModel>>();

        CreateMap<AddGroupModel, Package>();
        CreateMap<UpdateGroupModel, Package>();
    }
}