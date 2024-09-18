namespace DataAccess.EF.Concrete;

public class LessonGroupDal(HamsteraiDbContext context) : EfRepositoryBase<LessonGroup, HamsteraiDbContext>(context), ILessonGroupDal
{
}
