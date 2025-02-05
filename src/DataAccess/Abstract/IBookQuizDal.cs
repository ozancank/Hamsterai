namespace DataAccess.Abstract;

public interface IBookQuizDal : ISyncRepository<BookQuiz>, IAsyncRepository<BookQuiz>
{
}