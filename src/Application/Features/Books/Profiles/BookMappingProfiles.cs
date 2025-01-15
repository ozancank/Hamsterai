using Application.Features.Books.Models.Books;

namespace Application.Features.Books.Profiles;

public class BookMappingProfiles : Profile
{
    public BookMappingProfiles()
    {
        CreateMap<Book, GetBookModel>()
            .ForMember(dest => dest.SchoolName, opt => opt.MapFrom(src => src.School!.Name))
            .ForMember(dest => dest.LessonName, opt => opt.MapFrom(src => src.Lesson!.Name))
            .ForMember(dest => dest.PublisherName, opt => opt.MapFrom(src => src.Publisher!.Name))
            .ForMember(dest => dest.ClassRoomNames, opt => opt.MapFrom(src => src.BookClassRooms.Select(x => x.ClassRoom != null ? $"{x.ClassRoom.No} - {x.ClassRoom.Branch}" : string.Empty).ToList()));
        CreateMap<IPaginate<GetBookModel>, PageableModel<GetBookModel>>();

        CreateMap<AddBookModel, Book>();
    }
}