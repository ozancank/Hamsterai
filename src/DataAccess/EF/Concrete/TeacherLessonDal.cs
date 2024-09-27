namespace DataAccess.EF.Concrete;

public class TeacherLessonDal(HamsteraiDbContext context) : EfRepositoryBase<TeacherLesson, HamsteraiDbContext>(context), ITeacherLessonDal
{
}
