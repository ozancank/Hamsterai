namespace DataAccess.EF.Concrete;

public class RPackageLessonDal(HamsteraiDbContext context) : EfRepositoryBase<RPackageLesson, HamsteraiDbContext>(context), IRPackageLessonDal
{
}