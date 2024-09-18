using Business.Features.Auths.Models;
using Domain.Entities.Core;
using OCK.Core.Security.JWT;

namespace Business.Features.Auths.Profiles;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<AccessToken, AccessTokenModel>().ReverseMap();
        CreateMap<RefreshToken, RefreshTokenModel>().ReverseMap();
    }
}