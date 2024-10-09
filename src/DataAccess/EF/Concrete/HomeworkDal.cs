namespace DataAccess.EF.Concrete;

public class HomeworkDal(HamsteraiDbContext context) : EfRepositoryBase<Homework, HamsteraiDbContext>(context), IHomeworkDal
{
}
