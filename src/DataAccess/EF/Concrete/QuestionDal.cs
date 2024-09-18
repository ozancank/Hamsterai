namespace DataAccess.EF.Concrete;

public class QuestionDal(HamsteraiDbContext context) : EfRepositoryBase<Question, HamsteraiDbContext>(context), IQuestionDal
{
}