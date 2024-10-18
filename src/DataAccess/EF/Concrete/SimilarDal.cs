namespace DataAccess.EF.Concrete;

public class SimilarDal(HamsteraiDbContext context) : EfRepositoryBase<Similar, HamsteraiDbContext>(context), ISimilarDal
{
}