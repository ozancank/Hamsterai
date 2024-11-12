namespace DataAccess.Abstract;

public interface IQuestionDal : ISyncRepository<Question>, IAsyncRepository<Question>
{
}