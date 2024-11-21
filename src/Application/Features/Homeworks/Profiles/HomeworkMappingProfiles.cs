using Application.Features.Homeworks.Models;

namespace Application.Features.Homeworks.Profiles;

public class HomeworkMappingProfiles : Profile
{
    public HomeworkMappingProfiles()
    {
        CreateMap<Homework, GetHomeworkModel>()
            .ForMember(dest => dest.SchoolName, opt => opt.MapFrom(src => src.School != null ? src.School.Name : default))
            .ForMember(dest => dest.TeacherName, opt => opt.MapFrom(src => src.Teacher != null ? $"{src.Teacher.Name} {src.Teacher.Surname}" : "Sistem"))
            .ForMember(dest => dest.ClassName, opt => opt.MapFrom(src => src.ClassRoom != null ? $"{src.ClassRoom.No}-{src.ClassRoom.Branch}" : default))
            .ForMember(dest => dest.HomeworkStudents, opt => opt.MapFrom(src => src.HomeworkStudents));
        CreateMap<IPaginate<GetHomeworkModel>, PageableModel<GetHomeworkModel>>();

        CreateMap<Homework, GetHomeworkForStudentModel>()
            .ForMember(dest => dest.SchoolName, opt => opt.MapFrom(src => src.School != null ? src.School.Name : default))
            .ForMember(dest => dest.TeacherName, opt => opt.MapFrom(src => src.Teacher != null ? $"{src.Teacher.Name} {src.Teacher.Surname}" : "Sistem"))
            .ForMember(dest => dest.LessonName, opt => opt.MapFrom(src => src.Lesson != null ? src.Lesson.Name : default))
            .ForMember(dest => dest.ClassName, opt => opt.MapFrom(src => src.ClassRoom != null ? $"{src.ClassRoom.No}-{src.ClassRoom.Branch}" : default))
            .ForMember(dest => dest.FilePath, opt => opt.MapFrom(src => src.FilePath));
        CreateMap<IPaginate<GetHomeworkForStudentModel>, PageableModel<GetHomeworkForStudentModel>>();
    }
}