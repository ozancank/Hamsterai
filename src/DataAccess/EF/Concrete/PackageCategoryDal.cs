namespace DataAccess.EF.Concrete;

public class PackageCategoryDal(HamsteraiDbContext context) : EfRepositoryBase<PackageCategory, HamsteraiDbContext>(context), IPackageCategoryDal
{
}
