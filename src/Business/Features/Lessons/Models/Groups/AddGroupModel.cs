namespace Business.Features.Lessons.Models.Groups;

public sealed class AddGroupModel : IResponseModel
{
    public string Name { get; set; }
    public List<byte> GroupIds { get; set; }
}