namespace DataAccess.EF.Concrete;

public class HomeworkStudentDal(HamsteraiDbContext context) : EfRepositoryBase<HomeworkStudent, HamsteraiDbContext>(context), IHomeworkStudentDal
{
}