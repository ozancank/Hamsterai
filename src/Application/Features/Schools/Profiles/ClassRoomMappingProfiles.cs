﻿using Application.Features.Schools.Models.ClassRooms;

namespace Application.Features.Schools.Profiles;

public class ClassRoomMappingProfiles : Profile
{
    public ClassRoomMappingProfiles()
    {
        CreateMap<ClassRoom, GetClassRoomModel>()
            .ForMember(dest => dest.PackageName, opt => opt.MapFrom(src => src.Package != null ? src.Package.Name : default))
            .ForMember(dest => dest.School, opt => opt.MapFrom(src => src.School))
            .ForMember(dest => dest.Teachers, opt => opt.MapFrom(src => src.TeacherClassRooms.Select(x => x.Teacher)))
            .ForMember(dest => dest.Students, opt => opt.MapFrom(src => src.Students));
        CreateMap<IPaginate<GetClassRoomModel>, PageableModel<GetClassRoomModel>>();

        CreateMap<AddClassRoomModel, ClassRoom>();
        CreateMap<UpdateClassRoomModel, ClassRoom>();
    }
}