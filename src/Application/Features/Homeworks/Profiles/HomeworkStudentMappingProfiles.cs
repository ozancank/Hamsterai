using Application.Features.Homeworks.Models;

namespace Application.Features.Homeworks.Profiles;

public class HomeworkStudentMappingProfiles : Profile
{
    public HomeworkStudentMappingProfiles()
    {
        CreateMap<HomeworkStudent, GetHomeworkStudentModel>()
            .ForMember(dest => dest.TeacherName, opt => opt.MapFrom(src => src.Homework != null && src.Homework.Teacher != null ? $"{src.Homework.Teacher.Name} {src.Homework.Teacher.Surname}" : default))
            .ForMember(dest => dest.StudentName, opt => opt.MapFrom(src => src.Student != null ? $"{src.Student.Name} {src.Student.Surname}" : default))
            .ForMember(dest => dest.Homework, opt => opt.MapFrom(src => src.Homework));
        CreateMap<IPaginate<GetHomeworkStudentModel>, PageableModel<GetHomeworkStudentModel>>();
    }
}