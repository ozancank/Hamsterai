namespace DataAccess.EF.Concrete;

public class SchoolDal(HamsteraiDbContext context) : EfRepositoryBase<School, HamsteraiDbContext>(context), ISchoolDal
{
}
