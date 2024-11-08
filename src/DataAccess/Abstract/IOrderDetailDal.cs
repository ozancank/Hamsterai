namespace DataAccess.Abstract;

public interface IOrderDetailDal : ISyncRepository<OrderDetail>, IAsyncRepository<OrderDetail>
{
}
