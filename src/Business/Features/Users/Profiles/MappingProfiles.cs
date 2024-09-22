using Business.Features.Users.Models.Claim;
using Business.Features.Users.Models.User;
using Domain.Entities.Core;

namespace Business.Features.Users.Profiles;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<User, GetUserModel>()
            .ForMember(dest => dest.OperationClaims, opt => opt.MapFrom(src => src.UserOperationClaims.Select(o => o.OperationClaim!.Name).ToList()))
            .ForMember(dest => dest.ProfileFileName, opt => opt.MapFrom(src => src.ProfileUrl));

        CreateMap<IPaginate<GetUserModel>, PageableModel<GetUserModel>>();

        CreateMap<OperationClaim, GetOperationClaimListModel>();
        CreateMap<IPaginate<OperationClaim>, PageableModel<GetOperationClaimListModel>>();
    }
}