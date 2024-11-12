namespace DataAccess.EF.Concrete;

public class GainDal(HamsteraiDbContext context) : EfRepositoryBase<Gain, HamsteraiDbContext>(context), IGainDal
{
}