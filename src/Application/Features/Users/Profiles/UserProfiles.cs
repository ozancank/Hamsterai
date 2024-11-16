using Application.Features.Users.Models.Claim;
using Application.Features.Users.Models.User;
using Domain.Entities.Core;

namespace Application.Features.Users.Profiles;

public class UserProfiles : Profile
{
    public UserProfiles()
    {
        CreateMap<User, GetUserModel>()
            .ForMember(dest => dest.OperationClaims, opt => opt.MapFrom(src => src.UserOperationClaims.Select(o => o.OperationClaim != null ? o.OperationClaim.Name : default).ToList()))
            .ForMember(dest => dest.ProfileFileName, opt => opt.MapFrom(src => src.ProfileUrl))
            .ForMember(dest => dest.Packages, opt => opt.MapFrom(src => src.PackageUsers.Select(o => o.Package)));
        CreateMap<IPaginate<GetUserModel>, PageableModel<GetUserModel>>();

        CreateMap<OperationClaim, GetOperationClaimListModel>();
        CreateMap<IPaginate<OperationClaim>, PageableModel<GetOperationClaimListModel>>();
    }
}