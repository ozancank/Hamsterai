using Application.Features.Payments.Models;
using Application.Services.CommonService;
using MediatR;
using OCK.Core.Pipelines.Authorization;

namespace Application.Features.Payments.Queries;

public class GetPaymentsByDynamicQuery : IRequest<PageableModel<GetPaymentModel>>, ISecuredRequest<UserTypes>
{
    public required PageRequest PageRequest { get; set; }
    public required Dynamic Dynamic { get; set; }

    public UserTypes[] Roles { get; } = [];
    public bool AllowByPass => false;
}

public class GetPaymentsByDynamicQueryHandler(IMapper mapper,
                                              ICommonService commonService,
                                              IPaymentDal paymentDal) : IRequestHandler<GetPaymentsByDynamicQuery, PageableModel<GetPaymentModel>>
{
    public async Task<PageableModel<GetPaymentModel>> Handle(GetPaymentsByDynamicQuery request, CancellationToken cancellationToken)
    {
        request.PageRequest ??= new PageRequest();
        request.Dynamic ??= new Dynamic();
        var schoolId = commonService.HttpSchoolId ?? 0;

        var payments = await paymentDal.GetPageListAsyncAutoMapperByDynamic<GetPaymentModel>(
            dynamic: request.Dynamic,
            defaultOrderColumnName: x => x.CreateDate,
            enableTracking: false,
            size: request.PageRequest.PageSize,
            index: request.PageRequest.Page,
            predicate: x => commonService.HttpUserType == UserTypes.Administator || x.UserId == commonService.HttpUserId,
            configurationProvider: mapper.ConfigurationProvider,
            cancellationToken: cancellationToken);

        var result = mapper.Map<PageableModel<GetPaymentModel>>(payments);
        return result;
    }
}