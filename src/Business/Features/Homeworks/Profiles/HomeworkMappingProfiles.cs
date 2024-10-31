using Business.Features.Homeworks.Models;

namespace Business.Features.Homeworks.Profiles;

public class HomeworkMappingProfiles : Profile
{
    public HomeworkMappingProfiles()
    {
        CreateMap<Homework, GetHomeworkModel>()
            .ForMember(dest => dest.SchoolName, opt => opt.MapFrom(src => src.School!.Name))
            .ForMember(dest => dest.TeacherName, opt => opt.MapFrom(src => $"{src.Teacher!.Name} {src.Teacher.Surname}"))
            .ForMember(dest => dest.ClassName, opt => opt.MapFrom(src => $"{src.ClassRoom!.No}-{src.ClassRoom.Branch}"))
            .ForMember(dest => dest.HomeworkStudents, opt => opt.MapFrom(src => src.HomeworkStudents));
        CreateMap<IPaginate<GetHomeworkModel>, PageableModel<GetHomeworkModel>>();

        CreateMap<Homework, GetHomeworkForStudentModel>()
            .ForMember(dest => dest.SchoolName, opt => opt.MapFrom(src => src.School!.Name))
            .ForMember(dest => dest.TeacherName, opt => opt.MapFrom(src => $"{src.Teacher!.Name} {src.Teacher.Surname}"))
            .ForMember(dest => dest.LessonName, opt => opt.MapFrom(src => src.Lesson!.Name))
            .ForMember(dest => dest.ClassName, opt => opt.MapFrom(src => $"{src.ClassRoom!.No}-{src.ClassRoom.Branch}"))
            .ForMember(dest => dest.FilePath, opt => opt.MapFrom(src => src.FilePath));
        CreateMap<IPaginate<GetHomeworkForStudentModel>, PageableModel<GetHomeworkForStudentModel>>();
    }
}