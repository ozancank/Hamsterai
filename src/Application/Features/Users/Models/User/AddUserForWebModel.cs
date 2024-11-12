namespace Application.Features.Users.Models.User;

public sealed class AddUserForWebModel : IRequestModel
{
    public string? Email { get; set; }
    public string? Password { get; set; }
    public string? Name { get; set; }
    public string? Surname { get; set; }
    public string? Phone { get; set; }
    public string? TaxNumber { get; set; }
}