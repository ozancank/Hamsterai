namespace DataAccess.EF.Concrete;

public class BookQuizDal(HamsteraiDbContext context) : EfRepositoryBase<BookQuiz, HamsteraiDbContext>(context), IBookQuizDal
{
}