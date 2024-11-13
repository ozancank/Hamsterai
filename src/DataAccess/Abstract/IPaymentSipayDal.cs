namespace DataAccess.Abstract;

public interface IPaymentSipayDal : ISyncRepository<PaymentSipay>, IAsyncRepository<PaymentSipay>
{
}