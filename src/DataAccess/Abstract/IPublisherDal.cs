namespace DataAccess.Abstract;

public interface IPublisherDal : ISyncRepository<Publisher>, IAsyncRepository<Publisher>
{
}