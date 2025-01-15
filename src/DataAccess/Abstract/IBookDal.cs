namespace DataAccess.Abstract;

public interface IBookDal : ISyncRepository<Book>, IAsyncRepository<Book>
{
}