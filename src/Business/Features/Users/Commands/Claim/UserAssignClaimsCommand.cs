using Business.Features.Users.Rules;
using DataAccess.Abstract.Core;
using Domain.Entities.Core;
using MediatR;
using OCK.Core.Pipelines.Authorization;
using OCK.Core.Pipelines.Logging;

namespace Business.Features.Users.Commands.Claim;

public class UserAssignClaimsCommand : IRequest<bool>, ISecuredRequest<UserTypes>, ILoggableRequest
{
    public long Id { get; set; }
    public string[] AssignRoles { get; set; }

    public UserTypes[] Roles { get; } = [UserTypes.Administator];
    public string[] HidePropertyNames { get; } = [];
}

public class UserAssignClaimsCommandHandler(IUserDal userDal,
                                            IOperationClaimDal operationClaimDal,
                                            IUserOperationClaimDal userOperationClaimDal,
                                            UserRules userRules) : IRequestHandler<UserAssignClaimsCommand, bool>
{
    public async Task<bool> Handle(UserAssignClaimsCommand request, CancellationToken cancellationToken)
    {
        var user = await userDal.GetAsync(
            predicate: x => x.Id == request.Id,
            include: x => x.Include(u => u.UserOperationClaims).ThenInclude(u => u.OperationClaim),
            cancellationToken: cancellationToken);

        await userRules.UserShouldExistsAndActiveById(request.Id);
        await userRules.UserCanNotEditAtAdminUser(request.Id);
        await UserRules.AdminCanNotAssignClaim(request.AssignRoles);

        await userOperationClaimDal.DeleteRangeAsync(user.UserOperationClaims, cancellationToken);

        if (request.AssignRoles != null && request.AssignRoles.Length != 0)
        {
            var operationClaims = await operationClaimDal.GetListAsync(cancellationToken: cancellationToken);
            await UserRules.AssignRolesShouldBeRecordInDatabase(request.AssignRoles, operationClaims);
            var claims = new List<UserOperationClaim>();

            foreach (var role in request.AssignRoles)
            {
                await AddClaims(operationClaims, claims, request.Id, role);
            }

            foreach (var claim in claims)
            {
                var id = await userOperationClaimDal.GetNextPrimaryKeyAsync(x => x.Id, cancellationToken: cancellationToken);
                claim.Id = id;
                await userOperationClaimDal.AddAsync(claim, cancellationToken: cancellationToken);
            }
        }

        return true;
    }

    private static async Task AddClaims(IEnumerable<OperationClaim> operationClaims, List<UserOperationClaim> claims, long userId, string role)
    {
        var claim = operationClaims.FirstOrDefault(x => x.Name == role);
        if (claim == null) return;
        if (claim.ParentId > 0)
            await AddClaims(operationClaims, claims, userId, operationClaims.First(x => x.Id == claim.ParentId).Name);
        if (!claims.Any(x => x.OperationClaimId == claim.Id))
            claims.Add(new() { UserId = userId, OperationClaimId = claim.Id });
    }
}

public class AssignClaimsCommandHandlerHandlerValidator : AbstractValidator<UserAssignClaimsCommand>
{
    public AssignClaimsCommandHandlerHandlerValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage(Strings.IdNotEmpty);

        RuleFor(x => x.AssignRoles).NotNull().WithMessage(Strings.UserAssignClaimsNotNull);
    }
}