namespace DataAccess.EF.Concrete;

public class QuizQuestionDal(HamsteraiDbContext context) : EfRepositoryBase<QuizQuestion, HamsteraiDbContext>(context), IQuizQuestionDal
{
}