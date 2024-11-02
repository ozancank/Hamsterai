namespace DataAccess.EF.Concrete;

public class OrderDal(HamsteraiDbContext context) : EfRepositoryBase<Order, HamsteraiDbContext>(context), IOrderDal
{
}
