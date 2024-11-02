namespace DataAccess.Abstract;

public interface IOrderDal : ISyncRepository<Order>, IAsyncRepository<Order>
{
}
