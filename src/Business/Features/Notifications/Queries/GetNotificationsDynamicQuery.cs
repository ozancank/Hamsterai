using Business.Features.Notifications.Models.Notification;
using Business.Services.CommonService;
using MediatR;
using OCK.Core.Pipelines.Authorization;

namespace Business.Features.Notifications.Queries;

public class GetNotificationsDynamicQuery : IRequest<PageableModel<GetNotificationModel>>, ISecuredRequest<UserTypes>
{
    public PageRequest PageRequest { get; set; }
    public Dynamic Dynamic { get; set; }

    public UserTypes[] Roles { get; } = [];
}

public class GetNotificationsByDynamicQueryHandler(IMapper mapper,
                                                   INotificationDal notificationDal,
                                                   ICommonService commonService) : IRequestHandler<GetNotificationsDynamicQuery, PageableModel<GetNotificationModel>>
{
    public async Task<PageableModel<GetNotificationModel>> Handle(GetNotificationsDynamicQuery request, CancellationToken cancellationToken)
    {
        request.PageRequest ??= new PageRequest();

        var notification = await notificationDal.GetPageListAsyncAutoMapperByDynamic<GetNotificationModel>(
            dynamic: request.Dynamic,
            enableTracking: false,
            index: request.PageRequest.Page,
            size: request.PageRequest.PageSize,
            predicate: x => x.ReceiveredUserId == commonService.HttpUserId && x.IsActive,
            include: x => x.Include(u => u.SenderUser).Include(u => u.ReceiveredUser),
            defaultOrderColumnName: x => x.CreateDate,
            defaultDescending: true,
            configurationProvider: mapper.ConfigurationProvider,
            cancellationToken: cancellationToken);

        var result = mapper.Map<PageableModel<GetNotificationModel>>(notification);
        return result;
    }
}