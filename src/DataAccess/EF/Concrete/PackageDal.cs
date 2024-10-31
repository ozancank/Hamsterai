namespace DataAccess.EF.Concrete;

public class PackageDal(HamsteraiDbContext context) : EfRepositoryBase<Package, HamsteraiDbContext>(context), IPackageDal
{
}
