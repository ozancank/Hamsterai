namespace Application.Features.Books.Models.Books;

public class UpdateBookClassRoomModel : IRequestModel
{
    public int Id { get; set; }
    public int SchoolId { get; set; }
    public List<int> ClassRoomIds { get; set; } = [];
}