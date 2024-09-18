using OCK.Core.Security.Entities;

namespace Domain.Constants;

public struct OperationClaims
{
    public const string ByPass = "ByPass";
    public const string Admin = "Admin";
}

public class OperationClaimContainer
{
    public static ReadOnlySpan<OperationClaim> OperationClaimList => new([
            new(1, OperationClaims.Admin, "Admin", 0)
        ]);
}