namespace DataAccess.EF.Concrete;

public class PaymentDal(HamsteraiDbContext context) : EfRepositoryBase<Payment, HamsteraiDbContext>(context), IPaymentDal
{
}