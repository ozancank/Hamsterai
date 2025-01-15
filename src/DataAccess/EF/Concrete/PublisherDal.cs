namespace DataAccess.EF.Concrete;

public class PublisherDal(HamsteraiDbContext context) : EfRepositoryBase<Publisher, HamsteraiDbContext>(context), IPublisherDal
{
}