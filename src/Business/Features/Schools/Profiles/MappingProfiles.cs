using Business.Features.Schools.Models.Schools;

namespace Business.Features.Schools.Profiles;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<School, GetSchoolModel>();
        CreateMap<IPaginate<GetSchoolModel>, PageableModel<GetSchoolModel>>();

        CreateMap<AddSchoolModel, School>();
        CreateMap<UpdateSchoolModel, School>();
    }
}