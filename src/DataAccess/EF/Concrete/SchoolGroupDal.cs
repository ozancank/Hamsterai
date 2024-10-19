namespace DataAccess.EF.Concrete;

public class SchoolGroupDal(HamsteraiDbContext context) : EfRepositoryBase<SchoolGroup, HamsteraiDbContext>(context), ISchoolGroupDal
{
}
