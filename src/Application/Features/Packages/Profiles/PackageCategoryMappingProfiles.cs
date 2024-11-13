using Application.Features.Packages.Models.PackageCategories;

namespace Application.Features.Packages.Profiles;

public class PackageCategoryMappingProfiles : Profile
{
    public PackageCategoryMappingProfiles()
    {
        CreateMap<PackageCategory, GetPackageCategoryModel>()
            .ForMember(dest => dest.Packages, opt => opt.MapFrom(src => src.Packages.Where(x => x.IsActive).OrderBy(x => x!.SortNo).ThenBy(x => x!.Name)))
            .ForMember(dest => dest.PackageIds, opt => opt.MapFrom(src => src.Packages.Where(x => x.IsActive).OrderBy(x => x.SortNo).ThenBy(x => x.Name).Select(x => x.Id)));
        CreateMap<PackageCategory, GetPackageCategoryLiteModel>();
        CreateMap<IPaginate<GetPackageCategoryModel>, PageableModel<GetPackageCategoryModel>>();

        CreateMap<AddPackageCategoryModel, PackageCategory>();
        CreateMap<UpdatePackageCategoryModel, PackageCategory>();
    }
}