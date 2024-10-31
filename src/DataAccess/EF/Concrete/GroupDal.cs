namespace DataAccess.EF.Concrete;

public class GroupDal(HamsteraiDbContext context) : EfRepositoryBase<Package, HamsteraiDbContext>(context), IGroupDal
{
}
