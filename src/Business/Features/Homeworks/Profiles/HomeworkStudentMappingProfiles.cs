using Business.Features.Homeworks.Models;

namespace Business.Features.Homeworks.Profiles;

public class HomeworkStudentMappingProfiles : Profile
{
    public HomeworkStudentMappingProfiles()
    {
        CreateMap<HomeworkStudent, GetHomeworkStudentModel>()
            .ForMember(dest => dest.TeacherName, opt => opt.MapFrom(src => $"{src.Homework!.Teacher!.Name} {src.Homework.Teacher.Surname}"))
            .ForMember(dest => dest.StudentName, opt => opt.MapFrom(src => $"{src.Student!.Name} {src.Student.Surname}"))
            .ForMember(dest => dest.Homework, opt => opt.MapFrom(src => src.Homework));
        CreateMap<IPaginate<GetHomeworkStudentModel>, PageableModel<GetHomeworkStudentModel>>();
    }
}