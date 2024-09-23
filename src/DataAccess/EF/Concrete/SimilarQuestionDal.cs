namespace DataAccess.EF.Concrete;

public class SimilarQuestionDal(HamsteraiDbContext context) : EfRepositoryBase<Similar, HamsteraiDbContext>(context), ISimilarQuestionDal
{
}