namespace DataAccess.EF.Concrete;

public class RPackageSchoolDal(HamsteraiDbContext context) : EfRepositoryBase<RPackageSchool, HamsteraiDbContext>(context), IRPackageSchoolDal
{
}
