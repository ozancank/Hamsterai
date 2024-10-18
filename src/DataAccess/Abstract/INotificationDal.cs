namespace DataAccess.Abstract;

public interface INotificationDal : ISyncRepository<Notification>, IAsyncRepository<Notification>
{
}