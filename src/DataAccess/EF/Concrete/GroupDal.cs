namespace DataAccess.EF.Concrete;

public class GroupDal(HamsteraiDbContext context) : EfRepositoryBase<Group, HamsteraiDbContext>(context), IGroupDal
{
}
