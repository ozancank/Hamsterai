using Business.Features.Students.Models;

namespace Business.Features.Students.Profiles;

public class StudentMappingProfiles : Profile
{
    public StudentMappingProfiles()
    {
        CreateMap<Student, GetStudentModel>();
        CreateMap<IPaginate<GetStudentModel>, PageableModel<GetStudentModel>>();

        CreateMap<AddStudentModel, Student>();
        CreateMap<UpdateStudentModel, Student>();
    }
}