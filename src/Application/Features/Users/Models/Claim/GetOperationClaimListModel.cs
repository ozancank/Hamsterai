namespace Application.Features.Users.Models.Claim;

public sealed class GetOperationClaimListModel : IResponseModel
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public int ParentId { get; set; }
}