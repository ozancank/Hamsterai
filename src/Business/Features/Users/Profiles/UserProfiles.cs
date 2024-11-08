using Business.Features.Users.Models.Claim;
using Business.Features.Users.Models.User;
using Domain.Entities.Core;

namespace Business.Features.Users.Profiles;

public class UserProfiles : Profile
{
    public UserProfiles()
    {
        CreateMap<User, GetUserModel>()
            .ForMember(dest => dest.OperationClaims, opt => opt.MapFrom(src => src.UserOperationClaims.Select(o => o.OperationClaim!.Name).ToList()))
            .ForMember(dest => dest.ProfileFileName, opt => opt.MapFrom(src => src.ProfileUrl))
            .ForMember(dest => dest.Packages, opt => opt.MapFrom(src => src.PackageUsers.Select(o => o.Package).ToList()));
        ;

        CreateMap<IPaginate<GetUserModel>, PageableModel<GetUserModel>>();

        CreateMap<OperationClaim, GetOperationClaimListModel>();
        CreateMap<IPaginate<OperationClaim>, PageableModel<GetOperationClaimListModel>>();
    }
}