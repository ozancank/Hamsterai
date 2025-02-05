namespace DataAccess.EF.Concrete;

public class BookQuizUserDal(HamsteraiDbContext context) : EfRepositoryBase<BookQuizUser, HamsteraiDbContext>(context), IBookQuizUserDal
{
}