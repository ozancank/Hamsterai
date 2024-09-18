namespace DataAccess.EF.Concrete;

public class NotificationDeviceTokenDal(HamsteraiDbContext context) : EfRepositoryBase<NotificationDeviceToken, HamsteraiDbContext>(context), INotificationDeviceTokenDal
{
}