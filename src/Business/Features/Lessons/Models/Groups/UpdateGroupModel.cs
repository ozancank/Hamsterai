namespace Business.Features.Lessons.Models.Groups;

public sealed class UpdateGroupModel : IResponseModel
{
    public byte Id { get; set; }
    public string Name { get; set; }
}