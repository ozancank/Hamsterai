namespace DataAccess.Abstract;

public interface IPaymentDal : ISyncRepository<Payment>, IAsyncRepository<Payment>
{
}