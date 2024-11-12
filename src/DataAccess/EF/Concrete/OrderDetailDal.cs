namespace DataAccess.EF.Concrete;

public class OrderDetailDal(HamsteraiDbContext context) : EfRepositoryBase<OrderDetail, HamsteraiDbContext>(context), IOrderDetailDal
{
}