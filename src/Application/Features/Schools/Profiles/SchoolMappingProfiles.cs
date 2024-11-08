using Application.Features.Schools.Models.Schools;

namespace Application.Features.Schools.Profiles;

public class SchoolMappingProfiles : Profile
{
    public SchoolMappingProfiles()
    {
        CreateMap<School, GetSchoolModel>()
            .ForMember(dest => dest.TeacherCount, opt => opt.MapFrom(src => src.Users.Count(x => x.Type == UserTypes.Teacher)))
            .ForMember(dest => dest.StudentCount, opt => opt.MapFrom(src => src.Users.Count(x => x.Type == UserTypes.Student)))
            .ForMember(dest => dest.PackageIds, opt => opt.MapFrom(src => src.RPackageSchools.Select(x => x.PackageId)));
            //.ForMember(dest => dest.Packages, opt => opt.MapFrom(src => src.RPackageSchools.Select(x => x.Package)));
        CreateMap<IPaginate<GetSchoolModel>, PageableModel<GetSchoolModel>>();

        CreateMap<AddSchoolModel, School>();
        CreateMap<UpdateSchoolModel, School>();
    }
}