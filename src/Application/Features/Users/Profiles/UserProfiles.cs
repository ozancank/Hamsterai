using Application.Features.Users.Models.Claim;
using Application.Features.Users.Models.User;
using Domain.Entities.Core;
using System.Linq;

namespace Application.Features.Users.Profiles;

public class UserProfiles : Profile
{
    public UserProfiles()
    {
        CreateMap<User, GetUserModel>()
            .ForMember(dest => dest.OperationClaims, opt => opt.MapFrom(src => src.UserOperationClaims.Select(o => o.OperationClaim != null ? o.OperationClaim.Name : default).ToList()))
            .ForMember(dest => dest.ProfileFileName, opt => opt.MapFrom(src => src.ProfileUrl))
            .ForMember(dest => dest.TotalCredit, opt => opt.MapFrom(src => src.PackageUsers.DefaultIfEmpty().Sum(o => o != null ? o.QuestionCredit : default)))
            .ForMember(dest => dest.QuestionCredit, opt => opt.MapFrom(src => src.Questions.Count(x => AppStatics.QuestionStatusesForCredit.Contains(x.Status))))
            .ForMember(dest => dest.Packages, opt => opt.MapFrom(src => src.PackageUsers.Select(o => o.Package)))
            .ForMember(dest => dest.LicenceEndDate, opt => opt.MapFrom(src => src.PackageUsers.DefaultIfEmpty().Max(o => o != null ? o.EndDate : AppStatics.MilleniumDate)));
        CreateMap<IPaginate<GetUserModel>, PageableModel<GetUserModel>>();

        CreateMap<OperationClaim, GetOperationClaimListModel>();
        CreateMap<IPaginate<OperationClaim>, PageableModel<GetOperationClaimListModel>>();
    }
}