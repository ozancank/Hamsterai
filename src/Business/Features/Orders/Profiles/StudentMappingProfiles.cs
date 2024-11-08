//using Business.Features.Students.Models;

//namespace Business.Features.Students.Profiles;

//public class StudentMappingProfiles : Profile
//{
//    public StudentMappingProfiles()
//    {
//        CreateMap<Student, GetStudentModel>()
//            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.Name} {src.Surname}"))
//            .ForMember(dest => dest.ClassName, opt => opt.MapFrom(src => $"{src.ClassRoom!.No}-{src.ClassRoom.Branch}"))
//            .ForMember(dest => dest.TeacherNames, opt => opt.MapFrom(src => src.ClassRoom!.TeacherClassRooms.Select(x => $"{x.Teacher!.Name} {x.Teacher.Surname}")));
//        CreateMap<IPaginate<GetStudentModel>, PageableModel<GetStudentModel>>();

//        CreateMap<AddStudentModel, Student>();
//        CreateMap<UpdateStudentModel, Student>();
//    }
//}