namespace DataAccess.Abstract;

public interface IHomeworkDal : ISyncRepository<Homework>, IAsyncRepository<Homework>
{
}
