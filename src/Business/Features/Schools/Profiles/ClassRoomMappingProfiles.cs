using Business.Features.Schools.Models.ClassRooms;

namespace Business.Features.Schools.Profiles;

public class ClassRoomMappingProfiles : Profile
{
    public ClassRoomMappingProfiles()
    {
        CreateMap<ClassRoom, GetClassRoomModel>();
        CreateMap<IPaginate<GetClassRoomModel>, PageableModel<GetClassRoomModel>>();

        CreateMap<AddClassRoomModel, ClassRoom>();
        CreateMap<UpdateClassRoomModel, ClassRoom>();
    }
}