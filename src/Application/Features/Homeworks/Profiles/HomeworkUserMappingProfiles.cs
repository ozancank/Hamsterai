using Application.Features.Homeworks.Models;

namespace Application.Features.Homeworks.Profiles;

public class HomeworkUserMappingProfiles : Profile
{
    public HomeworkUserMappingProfiles()
    {
        CreateMap<HomeworkUser, GetHomeworkStudentModel>()
            .ForMember(dest => dest.StudentId, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.TeacherName, opt => opt.MapFrom(src => "Sistem"))
            .ForMember(dest => dest.StudentName, opt => opt.MapFrom(src => src.User != null ? $"{src.User.Name} {src.User.Surname}" : default))
            .ForMember(dest => dest.Homework, opt => opt.MapFrom(src => src.Homework))
            .ForMember(dest => dest.StudentId, opt => opt.MapFrom(src => src.UserId));
    }
}