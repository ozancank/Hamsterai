using Business.Features.Lessons.Models.Groups;
using Business.Features.Lessons.Models.Lessons;

namespace Business.Features.Lessons.Profiles;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        #region Lesson

        CreateMap<Lesson, GetLessonModel>()
            .ForMember(dest => dest.Groups, opt => opt.MapFrom(src => src.LessonGroups.Where(x => x.Group.IsActive).Select(x => x.Group).OrderBy(x => x.Name)));
        CreateMap<Lesson, GetLessonLiteModel>();
        CreateMap<IPaginate<GetLessonModel>, PageableModel<GetLessonModel>>();

        CreateMap<AddLessonModel, Lesson>();
        CreateMap<UpdateLessonModel, Lesson>();

        #endregion Lesson

        #region Group

        CreateMap<Group, GetGroupModel>()
            .ForMember(dest => dest.Lessons, opt => opt.MapFrom(src => src.LessonGroups.Where(x => x.Lesson.IsActive).Select(x => x.Lesson).OrderBy(x => x.Name)));
        CreateMap<Group, GetGroupLiteModel>();
        CreateMap<IPaginate<GetGroupModel>, PageableModel<GetGroupModel>>();

        CreateMap<AddGroupModel, Group>();
        CreateMap<UpdateGroupModel, Group>();

        #endregion Group
    }
}