namespace DataAccess.EF.Concrete;

public class TeacherDal(HamsteraiDbContext context) : EfRepositoryBase<Teacher, HamsteraiDbContext>(context), ITeacherDal
{
}