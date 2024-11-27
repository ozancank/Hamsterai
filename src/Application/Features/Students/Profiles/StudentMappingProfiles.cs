using Application.Features.Students.Models;

namespace Application.Features.Students.Profiles;

public class StudentMappingProfiles : Profile
{
    public StudentMappingProfiles()
    {
        CreateMap<Student, GetStudentModel>()
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.Name} {src.Surname}"))
            .ForMember(dest => dest.ClassName, opt => opt.MapFrom(src => src.ClassRoom != null ? $"{src.ClassRoom.No}-{src.ClassRoom.Branch}" : default))
            .ForMember(dest => dest.TeacherNames, opt => opt.MapFrom(src => src.ClassRoom != null ? src.ClassRoom.TeacherClassRooms.Select(x => src.Teachers != null ? $"{x.Teacher!.Name} {x.Teacher.Surname}" : default) : default));
        CreateMap<IPaginate<GetStudentModel>, PageableModel<GetStudentModel>>();

        CreateMap<Student, GetStudentLiteModel>()
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.Name} {src.Surname}"));
        CreateMap<IPaginate<GetStudentLiteModel>, PageableModel<GetStudentLiteModel>>();

        CreateMap<AddStudentModel, Student>();
        CreateMap<UpdateStudentModel, Student>();
    }
}