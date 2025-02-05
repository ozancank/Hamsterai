namespace DataAccess.Abstract;

public interface IBookQuizUserDal : ISyncRepository<BookQuizUser>, IAsyncRepository<BookQuizUser>
{
}