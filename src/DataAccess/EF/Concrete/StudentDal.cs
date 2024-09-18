namespace DataAccess.EF.Concrete;

public class StudentDal(HamsteraiDbContext context) : EfRepositoryBase<Student, HamsteraiDbContext>(context), IStudentDal
{
}
