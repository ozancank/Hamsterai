using Business.Features.Users.Models.Claim;
using DataAccess.Abstract.Core;
using MediatR;
using OCK.Core.Pipelines.Authorization;

namespace Business.Features.Users.Queries.Claims;

public class GetOperationClaimsQuery : IRequest<List<GetOperationClaimListModel>>, ISecuredRequest<UserTypes>
{
    public UserTypes[] Roles { get; } = [];
}

public class GetOperationClaimsQueryHandler(IMapper mapper, IOperationClaimDal operationClaimDal)
    : IRequestHandler<GetOperationClaimsQuery, List<GetOperationClaimListModel>>
{
    public async Task<List<GetOperationClaimListModel>> Handle(GetOperationClaimsQuery request, CancellationToken cancellationToken)
    {
        var operationClaims = await operationClaimDal.GetListAsyncAutoMapper<GetOperationClaimListModel>(predicate: x => x.Name != OperationClaims.Admin, orderBy: x => x.OrderBy(o => o.Name), configurationProvider: mapper.ConfigurationProvider, enableTracking: false, cancellationToken: cancellationToken);

        return operationClaims;
    }
}