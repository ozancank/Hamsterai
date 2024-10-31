namespace DataAccess.EF.Concrete;

public class RTeacherLessonDal(HamsteraiDbContext context) : EfRepositoryBase<RTeacherLesson, HamsteraiDbContext>(context), IRTeacherLessonDal
{
}
