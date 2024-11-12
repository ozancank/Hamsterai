namespace DataAccess.EF.Concrete;

public class LessonDal(HamsteraiDbContext context) : EfRepositoryBase<Lesson, HamsteraiDbContext>(context), ILessonDal
{
}