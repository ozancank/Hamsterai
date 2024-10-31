namespace DataAccess.EF.Concrete;

public class RPackageUserDal(HamsteraiDbContext context) : EfRepositoryBase<RPackageUser, HamsteraiDbContext>(context), IRPackageUserDal
{
}