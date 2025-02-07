namespace DataAccess.EF.Concrete;

public class HomeworkUserDal(HamsteraiDbContext context) : EfRepositoryBase<HomeworkUser, HamsteraiDbContext>(context), IHomeworkUserDal
{
}