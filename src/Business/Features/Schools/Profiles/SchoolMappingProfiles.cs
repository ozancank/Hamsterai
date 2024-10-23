using Business.Features.Schools.Models.Schools;

namespace Business.Features.Schools.Profiles;

public class SchoolMappingProfiles1 : Profile
{
    public SchoolMappingProfiles1()
    {
        CreateMap<School, GetSchoolModel>()
            .ForMember(dest => dest.TeacherCount, opt => opt.MapFrom(src => src.Users.Count(x => x.Type == UserTypes.Teacher)))
            .ForMember(dest => dest.StudentCount, opt => opt.MapFrom(src => src.Users.Count(x => x.Type == UserTypes.Student)))
            .ForMember(dest => dest.GroupIds, opt => opt.MapFrom(src => src.SchoolGroups.Select(x => x.GroupId)))
            .ForMember(dest => dest.Groups, opt => opt.MapFrom(src => src.SchoolGroups.Select(x => x.Group)));
        CreateMap<IPaginate<GetSchoolModel>, PageableModel<GetSchoolModel>>();

        CreateMap<AddSchoolModel, School>();
        CreateMap<UpdateSchoolModel, School>();
    }
}