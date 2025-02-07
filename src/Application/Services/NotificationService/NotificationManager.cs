using Application.Features.Notifications.Dto;
using Application.Services.CommonService;
using DataAccess.EF;
using Infrastructure.Notification;

namespace Application.Services.NotificationService;

public class NotificationManager(INotificationApi notificationApi,
                                 ICommonService commonService,
                                 IDbContextFactory<HamsteraiDbContext> contextFactory) : INotificationService
{
    public async Task<bool> PushNotificationAll(string title, string body, IReadOnlyDictionary<string, string> datas)
    {
        using var context = contextFactory.CreateDbContext();

        var date = DateTime.Now;

        datas ??= new Dictionary<string, string>();

        var tokens = await context.NotificationDeviceTokens.AsNoTracking().Include(x => x.User)
            .Where(x => x.IsActive && x.User!.IsActive)
            .Select(x => new { x.User!.Id, x.DeviceToken }).ToListAsync();

        if (tokens.Count == 0) return false;

        var message = new NotificationModel<string>()
        {
            Title = title,
            Body = body,
            Datas = datas,
            List = tokens.Select(x => x.DeviceToken)
        };

        await notificationApi.PushNotification(message);

        var notifications = tokens.Select(x => new Notification
        {
            Id = Guid.NewGuid(),
            IsActive = true,
            CreateDate = date,
            CreateUser = commonService.HttpUserId,
            UpdateDate = date,
            UpdateUser = commonService.HttpUserId,
            ReceiveredUserId = x.Id,
            IsRead = false,
            ReadDate = null,
            Title = title,
            Body = body,
            Type = NotificationTypes.Everbody,
            ReasonId = null
        }).ToList();

        await context.Notifications.AddRangeAsync(notifications);
        await context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> PushNotificationByUserId(NotificationUserDto dto)
    {
        using var context = contextFactory.CreateDbContext();

        var date = DateTime.Now;

        var users = await context.Users.Where(x => x.IsActive && dto.ReceivedUserId.Contains(x.Id)).ToListAsync();

        var notifications = users.Select(x => new Notification
        {
            Id = Guid.NewGuid(),
            IsActive = true,
            CreateDate = date,
            CreateUser = dto.SenderUserId,
            UpdateDate = date,
            UpdateUser = dto.SenderUserId,
            ReceiveredUserId = x.Id,
            IsRead = false,
            ReadDate = null,
            Title = dto.Title,
            Body = dto.Body,
            Type = dto.Type,
            ReasonId = dto.ReasonId
        }).ToList();

        await context.Notifications.AddRangeAsync(notifications);
        await context.SaveChangesAsync();

        var tokens = await context.NotificationDeviceTokens.AsNoTracking()
            .Include(x => x.User)
            .Where(x => dto.ReceivedUserId.Contains(x.UserId) && x.IsActive && x.User!.IsActive)
            .Select(x => new { x.User!.Id, x.DeviceToken })
            .ToListAsync();

        if (tokens.Count == 0) return false;

        var message = new NotificationModel<string>()
        {
            Title = dto.Title,
            Body = dto.Body,
            List = tokens.Select(x => x.DeviceToken)
        };

        await notificationApi.PushNotification(message);

        return true;
    }
}