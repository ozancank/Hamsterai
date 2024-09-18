namespace DataAccess.EF.Concrete;

public class SimilarQuestionDal(HamsteraiDbContext context) : EfRepositoryBase<SimilarQuestion, HamsteraiDbContext>(context), ISimilarQuestionDal
{
}