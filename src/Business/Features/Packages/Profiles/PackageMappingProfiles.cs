using Business.Features.Packages.Models;

namespace Business.Features.Packages.Profiles;

public class PackageMappingProfiles : Profile
{
    public PackageMappingProfiles()
    {
        CreateMap<Package, GetPackageModel>()
            .ForMember(dest => dest.Lessons, opt => opt.MapFrom(src => src.RPackageLessons.Where(x => x.Lesson!.IsActive).Select(x => x.Lesson).OrderBy(x => x!.Name)))
            .ForMember(dest => dest.LessonIds, opt => opt.MapFrom(src => src.RPackageLessons.Select(x => x.LessonId)));
        CreateMap<Package, GetPackageLiteModel>();
        CreateMap<IPaginate<GetPackageModel>, PageableModel<GetPackageModel>>();

        CreateMap<AddPackageModel, Package>();
        CreateMap<UpdatePackageModel, Package>();
    }
}