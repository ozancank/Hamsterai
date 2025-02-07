using Application.Features.Packages.Models.Packages;

namespace Application.Features.Packages.Profiles;

public class PackageMappingProfiles : Profile
{
    public PackageMappingProfiles()
    {
        CreateMap<Package, GetPackageModel>()
            .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.PackageCategory))
            .ForMember(dest => dest.Lessons, opt => opt.MapFrom(src => src.RPackageLessons.Where(x => x.IsActive && x.Lesson != null && x.Lesson.IsActive).Select(x => x.Lesson).OrderBy(x => x != null ? x.SortNo : default).ThenBy(x => x != null ? x.Name : default)))
            .ForMember(dest => dest.LessonIds, opt => opt.MapFrom(src => src.RPackageLessons.Where(x => x.IsActive && x.Lesson != null && x.Lesson.IsActive).Select(x => x.LessonId).OrderBy(x => x))).ReverseMap();
        CreateMap<Package, GetPackageLiteModel>()
            .ForMember(dest => dest.LessonIds, opt => opt.MapFrom(src => src.RPackageLessons.Where(x => x.IsActive && x.Lesson != null && x.Lesson.IsActive).Select(x => x.LessonId).OrderBy(x => x)));
        CreateMap<IPaginate<GetPackageModel>, PageableModel<GetPackageModel>>();

        CreateMap<AddPackageModel, Package>();
        CreateMap<UpdatePackageModel, Package>();
    }
}