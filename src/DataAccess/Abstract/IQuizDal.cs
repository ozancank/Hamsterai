namespace DataAccess.Abstract;

public interface IQuizDal : ISyncRepository<Quiz>, IAsyncRepository<Quiz>
{
}