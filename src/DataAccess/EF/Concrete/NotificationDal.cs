namespace DataAccess.EF.Concrete;

public class NotificationDal(HamsteraiDbContext context) : EfRepositoryBase<Notification, HamsteraiDbContext>(context), INotificationDal
{
}