using Business.Features.Schools.Models.Teachers;

namespace Business.Features.Schools.Profiles;

public class TeacherMappingProfiles : Profile
{
    public TeacherMappingProfiles()
    {
        CreateMap<Teacher, GetTeacherModel>()
            .ForMember(dest => dest.SchoolName, opt => opt.MapFrom(src => src.School.Name))
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.Name} {src.Surname}"))
            .ForMember(dest => dest.Lessons, opt => opt.MapFrom(src => src.TeacherLessons.Select(x => x.Lesson)))
            .ForMember(dest => dest.ClassRooms, opt => opt.MapFrom(src => src.TeacherClassRooms.Select(x => x.ClassRoom)));
        CreateMap<IPaginate<GetTeacherModel>, PageableModel<GetTeacherModel>>();

        CreateMap<AddTeacherModel, Teacher>();
        CreateMap<UpdateTeacherModel, Teacher>();
    }
}