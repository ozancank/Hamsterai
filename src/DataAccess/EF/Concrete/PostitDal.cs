namespace DataAccess.EF.Concrete;

public class PostitDal(HamsteraiDbContext context) : EfRepositoryBase<Postit, HamsteraiDbContext>(context), IPostitDal
{
}