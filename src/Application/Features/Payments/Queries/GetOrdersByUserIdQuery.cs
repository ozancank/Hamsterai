using Application.Features.Payments.Models;
using Application.Services.CommonService;
using MediatR;
using OCK.Core.Pipelines.Authorization;

namespace Application.Features.Payments.Queries;

public class GetPaymentsByUserIdQuery : IRequest<PageableModel<GetPaymentModel>>, ISecuredRequest<UserTypes>
{
    public required PageRequest PageRequest { get; set; }
    public long UserId { get; set; }

    public UserTypes[] Roles { get; } = [];
    public bool AllowByPass => false;
}

public class GetPaymentsByUserIdQueryHandler(IMapper mapper,
                                           ICommonService commonService,
                                           IPaymentDal orderDal) : IRequestHandler<GetPaymentsByUserIdQuery, PageableModel<GetPaymentModel>>
{
    public async Task<PageableModel<GetPaymentModel>> Handle(GetPaymentsByUserIdQuery request, CancellationToken cancellationToken)
    {
        var entities = await orderDal.GetPageListAsyncAutoMapper<GetPaymentModel>(
            enableTracking: false,
            index: request.PageRequest.Page,
            size: request.PageRequest.PageSize,
            predicate: x => x.UserId == request.UserId && (commonService.HttpUserType == UserTypes.Administator || x.UserId == commonService.HttpUserId),
            orderBy: x => x.OrderByDescending(x => x.CreateDate),
            configurationProvider: mapper.ConfigurationProvider,
            cancellationToken: cancellationToken);

        var result = mapper.Map<PageableModel<GetPaymentModel>>(entities);
        return result;
    }
}