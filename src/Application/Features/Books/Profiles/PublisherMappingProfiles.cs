using Application.Features.Books.Models.Publisher;

namespace Application.Features.Books.Profiles;

public class PublisherMappingProfiles : Profile
{
    public PublisherMappingProfiles()
    {
        CreateMap<Publisher, GetPublisherModel>();
        CreateMap<IPaginate<GetPublisherModel>, PageableModel<GetPublisherModel>>();

        CreateMap<AddPublisherModel, Publisher>();
        CreateMap<AddPublisherModel, Publisher>();
    }
}