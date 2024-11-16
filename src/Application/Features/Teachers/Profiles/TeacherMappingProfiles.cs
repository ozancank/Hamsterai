using Application.Features.Teachers.Models;

namespace Application.Features.Teachers.Profiles;

public class TeacherMappingProfiles : Profile
{
    public TeacherMappingProfiles()
    {
        CreateMap<Teacher, GetTeacherModel>()
            .ForMember(dest => dest.SchoolName, opt => opt.MapFrom(src => src.School != null ? src.School.Name : default))
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.Name} {src.Surname}"))
            .ForMember(dest => dest.Lessons, opt => opt.MapFrom(src => src.RTeacherLessons.Select(x => x.Lesson)))
            .ForMember(dest => dest.ClassRooms, opt => opt.MapFrom(src => src.RTeacherClassRooms.Select(x => x.ClassRoom)));
        CreateMap<IPaginate<GetTeacherModel>, PageableModel<GetTeacherModel>>();

        CreateMap<AddTeacherModel, Teacher>();
        CreateMap<UpdateTeacherModel, Teacher>();
    }
}