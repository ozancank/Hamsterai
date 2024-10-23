using Business.Features.Lessons.Models.Groups;

namespace Business.Features.Lessons.Profiles;

public class GroupMappingProfiles : Profile
{
    public GroupMappingProfiles()
    {
        CreateMap<Group, GetGroupModel>()
            .ForMember(dest => dest.Lessons, opt => opt.MapFrom(src => src.LessonGroups.Where(x => x.Lesson.IsActive).Select(x => x.Lesson).OrderBy(x => x.Name)))
            .ForMember(dest => dest.LessonIds, opt => opt.MapFrom(src => src.LessonGroups.Select(x => x.LessonId)));
        CreateMap<Group, GetGroupLiteModel>();
        CreateMap<IPaginate<GetGroupModel>, PageableModel<GetGroupModel>>();

        CreateMap<AddGroupModel, Group>();
        CreateMap<UpdateGroupModel, Group>();
    }
}