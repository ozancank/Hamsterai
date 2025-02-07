namespace Application.Features.Books.Models.Publisher;

public class UpdatePublisherModel : IRequestModel
{
    public short Id { get; set; }
    public string? Name { get; set; }
}