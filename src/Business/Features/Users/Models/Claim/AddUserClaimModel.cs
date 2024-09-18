namespace Business.Features.Users.Models.Claim;

public sealed class AddUserClaimModel : IRequestModel
{
    public long Id { get; set; }
    public List<string> Roles { get; set; }
}