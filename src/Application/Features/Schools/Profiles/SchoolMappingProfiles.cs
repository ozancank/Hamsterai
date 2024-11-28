using Application.Features.Schools.Models.Schools;
using OtpNet;

namespace Application.Features.Schools.Profiles;

public class SchoolMappingProfiles : Profile
{
    public SchoolMappingProfiles()
    {
        CreateMap<School, GetSchoolModel>()
            .ForMember(dest => dest.TeacherCount, opt => opt.MapFrom(src => src.Users.Count(x => x.Type == UserTypes.Teacher)))
            .ForMember(dest => dest.StudentCount, opt => opt.MapFrom(src => src.Users.Count(x => x.Type == UserTypes.Student)))
            .ForMember(dest => dest.PackageIds, opt => opt.MapFrom(src => src.Users.Where(x => x.Type == UserTypes.School).SelectMany(x => x.PackageUsers).Where(p => p.IsActive && p.Package != null && p.Package.IsActive).Select(p => p.Package != null ? p.Package.Id : (byte)0)))
            .ForMember(dest => dest.Packages, opt => opt.MapFrom(src => src.Users.Where(x => x.Type == UserTypes.School).SelectMany(x => x.PackageUsers).Where(p => p.IsActive && p.Package != null && p.Package.IsActive).Select(p => p.Package)))
            .ForMember(dest => dest.LicenseEndDate, opt => opt.MapFrom(src => src.Users.Where(x => x.Type == UserTypes.School).SelectMany(x => x.PackageUsers).Where(p => p.IsActive && p.Package != null && p.Package.IsActive).DefaultIfEmpty().Max(p => p != null ? p.EndDate : AppStatics.MilleniumDate)));
        CreateMap<IPaginate<GetSchoolModel>, PageableModel<GetSchoolModel>>();

        CreateMap<School, GetSchoolLiteModel>()
            .ForMember(dest => dest.TeacherCount, opt => opt.MapFrom(src => src.Users.Count(x => x.Type == UserTypes.Teacher)))
            .ForMember(dest => dest.StudentCount, opt => opt.MapFrom(src => src.Users.Count(x => x.Type == UserTypes.Student)))
            .ForMember(dest => dest.PackageIds, opt => opt.MapFrom(src => src.Users.Where(x => x.Type == UserTypes.School).SelectMany(x => x.PackageUsers).Where(p => p.IsActive && p.Package != null && p.Package.IsActive).Select(p => p.Package!.Id)));
        CreateMap<IPaginate<GetSchoolLiteModel>, PageableModel<GetSchoolLiteModel>>();

        CreateMap<AddSchoolModel, School>();
        CreateMap<UpdateSchoolModel, School>();
    }
}