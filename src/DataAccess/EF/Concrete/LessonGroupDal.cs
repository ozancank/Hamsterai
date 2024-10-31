namespace DataAccess.EF.Concrete;

public class LessonGroupDal(HamsteraiDbContext context) : EfRepositoryBase<RPackageGroup, HamsteraiDbContext>(context), ILessonGroupDal
{
}
