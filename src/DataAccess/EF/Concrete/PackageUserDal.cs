namespace DataAccess.EF.Concrete;

public class PackageUserDal(HamsteraiDbContext context) : EfRepositoryBase<PackageUser, HamsteraiDbContext>(context), IPackageUserDal
{
}