namespace DataAccess.EF.Concrete;

public class PaymentSipayDal(HamsteraiDbContext context) : EfRepositoryBase<PaymentSipay, HamsteraiDbContext>(context), IPaymentSipayDal;