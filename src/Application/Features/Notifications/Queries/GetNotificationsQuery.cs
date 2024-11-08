using Application.Features.Notifications.Models.Notification;
using Application.Services.CommonService;
using MediatR;
using OCK.Core.Pipelines.Authorization;

namespace Application.Features.Notifications.Queries;

public class GetNotificationsQuery : IRequest<PageableModel<GetNotificationModel>>, ISecuredRequest<UserTypes>
{
    public required PageRequest PageRequest { get; set; }

    public UserTypes[] Roles { get; } = [];
    public bool AllowByPass => false;
}

public class GetNotificationsQueryHandler(IMapper mapper,
                                          INotificationDal notificationDal,
                                          ICommonService commonServices) : IRequestHandler<GetNotificationsQuery, PageableModel<GetNotificationModel>>
{
    public async Task<PageableModel<GetNotificationModel>> Handle(GetNotificationsQuery request, CancellationToken cancellationToken)
    {
        request.PageRequest ??= new PageRequest();

        var notification = await notificationDal.GetPageListAsyncAutoMapper<GetNotificationModel>(
            enableTracking: false,
            predicate: x => x.ReceiveredUserId == commonServices.HttpUserId && x.IsActive,
            include: x => x.Include(u => u.SenderUser).Include(u => u.ReceiveredUser),
            orderBy: x => x.OrderByDescending(y => y.CreateDate),
            index: request.PageRequest.Page, size: request.PageRequest.PageSize,
            configurationProvider: mapper.ConfigurationProvider,
            cancellationToken: cancellationToken);

        var result = mapper.Map<PageableModel<GetNotificationModel>>(notification);
        return result;
    }
}