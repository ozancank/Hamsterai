namespace DataAccess.EF.Concrete;

public class QuizDal(HamsteraiDbContext context) : EfRepositoryBase<Quiz, HamsteraiDbContext>(context), IQuizDal
{
}
